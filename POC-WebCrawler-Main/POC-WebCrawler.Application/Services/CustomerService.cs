using Microsoft.Extensions.Configuration;
using POC_WebCrawler.Domain.Entities;
using POC_WebCrawler.Domain.Interfaces;
using System.Text;
using System.Text.Json;

namespace POC_WebCrawler.Application.Services
{
    public class CustomerService : ICustomerService
    {
        private readonly string _customerCacheKey;
        private readonly string _customerCpfQueueName;
        private readonly string _customerElasticIndex;
        private readonly IElasticSearchApiRepository _elasticApiRepository;
        private readonly IConfiguration _config;
        private readonly IRedisCacheRepository _cacheRepository;
        private readonly IRabbitMQProducer _producer;
        private readonly IRabbitMQConsumer _consumer;

        public CustomerService(
                IConfiguration config,
                IElasticSearchApiRepository elasticApiRepository,
                IRedisCacheRepository cacheRepository, 
                IRabbitMQProducer producer, 
                IRabbitMQConsumer consumer
            )
        {
            _config = config;
            _customerCacheKey = _config["RedisCachingSettings:CustomerCacheKey"];
            _customerCpfQueueName = _config["RabbitMQSettings:CpfQueueName"];
            _customerElasticIndex = _config["ElasticSettings:CustomerIndex"];
            _elasticApiRepository = elasticApiRepository;
            _cacheRepository = cacheRepository;
            _producer = producer;
            _consumer = consumer;
        }

        public async Task<List<string>> ConsumeListFromQueue()
        {
            try
            {
                var message = await _consumer.Consume(_customerCpfQueueName);

                if(string.IsNullOrEmpty(message))
                    return new List<string>();

                return DeserializeCpfListFromQueue(message);
            }
            catch(Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task FillCustomerQueue()
        {
            try
            {
                var projectPath = GetWebProjectPath();
                var filePath = Path.Combine(projectPath, "CustomerData.txt");
                var cpfList = JsonSerializer.Serialize(File.ReadAllLines(filePath));

                await _producer.Send(_customerCpfQueueName, cpfList);
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        public async Task CleanCustomerCache()
        {
            await _cacheRepository.Clean(_customerCacheKey);
        }

        public async Task StoreInCache(string jsonValue)
        {
            var cachedData = await GetCachedCustomersList();

            if (!cachedData.Any())
            {
                await _cacheRepository.Set(_customerCacheKey, jsonValue);
            }
            else
            {
                var newData = DeserializeCachedList(jsonValue);
                var filteredNewData = new List<Customer>();

                foreach(var item in newData)
                {
                    if(!cachedData.Any(x => x.Cpf == item.Cpf))
                        filteredNewData.Add(item);
                }

                if (filteredNewData.Any())
                {
                    cachedData.AddRange(filteredNewData);
                    await CleanCustomerCache();
                    await _cacheRepository.Set(_customerCacheKey, JsonSerializer.Serialize(cachedData));
                }
            }
        }

        public async Task<List<Customer>> GetCachedCustomersList()
        {
            var cachedData = await _cacheRepository.Get(_customerCacheKey);

            if (!string.IsNullOrEmpty(cachedData))
                return DeserializeCachedList(cachedData);

            return new List<Customer>();
        }

        public async Task<IEnumerable<Customer>> SearchCustomerDataByCpf(string cpf)
        {
            var result = await _elasticApiRepository.SearchDataByProperty<Customer>(_customerElasticIndex, "Cpf", cpf);

            if (result == null || !result.Any())
                throw new Exception("Customer not found.");

            return result;
        }

        public async Task IndexCustomerData(Customer customer)
        {
            var indexExists = await _elasticApiRepository.IndexExists(_customerElasticIndex);
            
            if (!indexExists)
                await CreateCustomerIndex(_customerElasticIndex);

            var documentExists = await _elasticApiRepository.DataExists<Customer>(customer.Cpf, _customerElasticIndex);

            if (!documentExists)
            {
                var result = await _elasticApiRepository.IndexData(_customerElasticIndex, customer, customer.Cpf);

                if (!result.IsValid)
                    throw new Exception(result.DebugInformation);
            }
        }

        public async Task CreateCustomerIndex(string index)
        {
            var indexExists = await _elasticApiRepository.IndexExists(index);

            if (indexExists)
                throw new Exception("Index already exists.");

            var result = await _elasticApiRepository.CreateIndex<Customer>(index);

            if (!result.IsValid)
                throw new Exception(result.DebugInformation);
        }

        private string GetWebProjectPath()
        {
            var builder = new StringBuilder();
            var solutionName = "POC-WebCrawler-Main";
            var webProjectName = "POC-WebCrawler.Web";

            var currentDomain = AppDomain.CurrentDomain.BaseDirectory;
            var solutionPath = currentDomain.Substring(0, currentDomain.IndexOf(solutionName) + solutionName.Length);

            builder.Append(solutionPath);
            builder.Append("\\");
            builder.Append(webProjectName);

            return builder.ToString();
        }

        private List<string> DeserializeCpfListFromQueue(string jsonValue)
        {
            var utf8Bytes = Encoding.UTF8.GetBytes(jsonValue);

            using (MemoryStream memoryStream = new MemoryStream(utf8Bytes))
            {
                return JsonSerializer.Deserialize<List<string>>(memoryStream);
            }
        }

        private List<Customer> DeserializeCachedList(string jsonValue)
        {
            using (MemoryStream cacheStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonValue)))
            {
                return JsonSerializer.Deserialize<List<Customer>>(cacheStream);
            }
        }
    }
}
