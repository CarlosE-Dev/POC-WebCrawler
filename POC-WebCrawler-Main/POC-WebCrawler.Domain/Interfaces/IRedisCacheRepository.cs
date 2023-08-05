namespace POC_WebCrawler.Domain.Interfaces
{
    public interface IRedisCacheRepository
    {
        Task<string> Get(string key);
        Task Set(string key, string jsonValue);
        Task Clean(string key);
    }
}
