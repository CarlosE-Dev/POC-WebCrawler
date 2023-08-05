using POC_WebCrawler.Domain.Entities;

namespace POC_WebCrawler.Domain.Interfaces
{
    public interface ICustomerService
    {
        Task<IEnumerable<Customer>> SearchCustomerDataByCpf(string cpf);
        Task IndexCustomerData(Customer customer);
        Task CreateCustomerIndex(string index);
        Task<List<Customer>> GetCachedCustomersList();
        Task StoreInCache(string jsonValue);
        Task CleanCustomerCache();
        Task FillCustomerQueue();
        Task<List<string>> ConsumeListFromQueue();
    }
}
