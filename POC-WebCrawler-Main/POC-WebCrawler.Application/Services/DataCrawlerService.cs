using Microsoft.Extensions.Configuration;
using POC_WebCrawler.Domain.Entities;
using POC_WebCrawler.Domain.Interfaces;
using System.Text.Json;

namespace POC_WebCrawler.Application.Services
{
    public class DataCrawlerService : IDataCrawlerService
    {
        private readonly IConfiguration _config;
        private readonly ICustomerService _customerService;
        private readonly IWebDriverCrawler _webDriverCrawler;

        public DataCrawlerService(
            IConfiguration config,
            ICustomerService customerService,
            IWebDriverCrawler webDriverCrawler
            )
        {
            _config = config;
            _customerService = customerService;
            _webDriverCrawler = webDriverCrawler;
        }

        public async Task<int> Execute()
        {
            var cpfListToSearch = await GetCpfList();
            if (!cpfListToSearch.Any())
                return 0;

            var results = await GetDataFromWebSite(cpfListToSearch);
            await IndexResults(results);
            await StoreInCache(results);

            return results.Count;
        }

        private async Task<List<Customer>> GetDataFromWebSite(List<string> dataToSearch)
        {
            var results = new List<Customer>();
            var firstData = dataToSearch.First();
            dataToSearch.Remove(firstData);

            await _webDriverCrawler.EnterWebSite(_config["WebsiteCredentials:Url"]);
            await _webDriverCrawler.Authenticate(_config["WebsiteCredentials:User"], _config["WebsiteCredentials:Password"]);
            await _webDriverCrawler.ClosePopupAfterLogin();
            await _webDriverCrawler.DownMenuPageScrollAfterLogin();
            await _webDriverCrawler.GoToSearchForm();
            await _webDriverCrawler.DownMenuPageScrollAfterOpenForm();
            await _webDriverCrawler.PerformSearch(firstData);
            await _webDriverCrawler.DownMenuPageScrollAfterSearch();
            var result = await _webDriverCrawler.CaptureResults();

            if(result.Any())
                results.Add(await CreateCustomer(firstData, result));

            if (dataToSearch.Count > 0)
                results.AddRange(await RetrySearch(dataToSearch));

            await _webDriverCrawler.CloseBrowser();
            
            return results;
        }

        private async Task StoreInCache(List<Customer> customers)
        {
            await _customerService.StoreInCache(JsonSerializer.Serialize(customers));
        }

        private async Task IndexResults(List<Customer> customers)
        {
            foreach(var customer in customers)
            {
                await _customerService.IndexCustomerData(customer);
            }
        }

        private async Task<List<Customer>> RetrySearch(List<string> dataToSearch)
        {
            var results = new List<Customer>();

            foreach(var data in dataToSearch)
            {
                await _webDriverCrawler.BackToForm();
                await _webDriverCrawler.DownMenuPageScrollAfterOpenForm();
                await _webDriverCrawler.PerformSearch(data);
                await _webDriverCrawler.DownMenuPageScrollAfterSearch();
                var result = await _webDriverCrawler.CaptureResults();
                
                if (result.Any())
                    results.Add(await CreateCustomer(data, result));
            }

            return results;
        }

        private async Task<Customer> CreateCustomer(string cpf, List<string> registerNumbers)
        {
            return await Task.FromResult(new Customer(cpf, registerNumbers));
        }

        private async Task<List<string>> GetCpfList()
        {
            await _customerService.FillCustomerQueue();
            var cpfList = await GetCpfListFromQueue();

            return await VerifyItemsExistsOnCache(cpfList);
        }

        private async Task<List<string>> GetCpfListFromQueue()
        {
            var result = await _customerService.ConsumeListFromQueue();

            if (!result.Any())
                throw new Exception("CPF list not found.");

            return result;
        }

        private async Task<List<string>> VerifyItemsExistsOnCache(List<string> cpfList)
        {
            var cachedList = await _customerService.GetCachedCustomersList();

            if (cachedList.Any())
            {
                var filteredList = new List<string>();

                foreach (var cpf in cpfList)
                {
                    if (cachedList.Any(c => c.Cpf == cpf))
                        continue;

                    filteredList.Add(cpf);
                }

                return filteredList;
            }
            else
            {
                return cpfList;
            }
        }
    }
}
