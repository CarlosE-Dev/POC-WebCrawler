namespace POC_WebCrawler.Domain.Interfaces
{
    public interface IWebDriverCrawler
    {
        Task EnterWebSite(string url);
        Task Authenticate(string user, string password);
        Task ClosePopupAfterLogin();
        Task DownMenuPageScrollAfterLogin();
        Task GoToSearchForm();
        Task DownMenuPageScrollAfterOpenForm();
        Task PerformSearch(string cpf);
        Task DownMenuPageScrollAfterSearch();
        Task<List<string>> CaptureResults();
        Task BackToForm();
        Task CloseBrowser();
    }
}
