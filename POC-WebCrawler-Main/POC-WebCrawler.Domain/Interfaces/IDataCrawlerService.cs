namespace POC_WebCrawler.Domain.Interfaces
{
    public interface IDataCrawlerService
    {
        Task<int> Execute();
    }
}
