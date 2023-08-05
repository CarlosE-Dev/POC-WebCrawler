using Microsoft.Extensions.Configuration;
using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using OpenQA.Selenium.Support.UI;
using POC_WebCrawler.Domain.Interfaces;
using SeleniumExtras.WaitHelpers;

namespace POC_WebCrawler.Infra.WebDriver
{
    public class WebDriverCrawler : IWebDriverCrawler
    {
        private readonly IConfiguration _config;
        private readonly IWebDriver _driver;
        private readonly IWait<IWebDriver> _wait;
        public WebDriverCrawler(
            IConfiguration config,
            IWebDriver driver
            )
        {
            _config = config;
            _driver = driver;
            _wait = new WebDriverWait(_driver, TimeSpan.FromSeconds(double.Parse(_config["CrawlerSettings:MaxWaitBeforeTimeoutGeneral"])));
        }

        public Task EnterWebSite(string url)
        {
            _driver.Navigate().GoToUrl(url);

            return Task.CompletedTask;
        }

        public Task Authenticate(string user, string password)
        {
            var frameset = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("frameset")));
            var frames = frameset.FindElements(By.TagName("frame"));
            var frame = frames.First();

            _driver.SwitchTo().Frame(frame);

            var userInputElement = _driver.FindElement(By.Id("user"));
            userInputElement.SendKeys(user);

            var passwordInputElement = _driver.FindElement(By.Id("pass"));
            passwordInputElement.SendKeys(password);

            var submitButtonElement = _driver.FindElement(By.Id("botao"));

            Thread.Sleep(500);
            submitButtonElement.Click();

            return Task.CompletedTask;
        }

        public Task ClosePopupAfterLogin()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("/html/body/app-root/app-home/ion-app/ion-menu/ion-header/ion-toolbar/ion-title")));
            var buttonTagElement = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("ion-button")));
            var shadowRoot = buttonTagElement.GetShadowRoot();
            var closeButton = shadowRoot.FindElement(By.CssSelector("[part=\"native\"]"));
            Actions actions = new Actions(_driver);
            
            Thread.Sleep(500);
            actions.MoveByOffset(closeButton.Location.X, closeButton.Location.Y).Perform();
            Thread.Sleep(500);
            actions.Click().Perform();
            Thread.Sleep(500);
            actions.Click().Perform();

            return Task.CompletedTask;
        }

        public Task DownMenuPageScrollAfterLogin()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"extratoonline\"]/ion-row[2]/ion-col/ion-card/ion-grid/ion-row[2]/ion-col/ion-card/ion-card-header/ion-card-title")));
            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("app-home")));
            var ionAppElement = element.FindElement(By.TagName("ion-app"));
            var mainElement = ionAppElement.FindElement(By.Id("main"));
            var ionContentElement = mainElement.FindElement(By.TagName("ion-content"));
            var appExtratoElement = ionContentElement.FindElement(By.TagName("app-extrato"));
            var ionContent2Element = appExtratoElement.FindElement(By.TagName("ion-content"));

            var shadowRoot = ionContent2Element.GetShadowRoot();
            var scrollElement = shadowRoot.FindElement(By.CssSelector("[part=\"scroll\"]"));

            ExecuteJsScript("arguments[0].scrollTop += 1000;", scrollElement);

            return Task.CompletedTask;
        }

        public Task GoToSearchForm()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"extratoonline\"]/ion-row[2]/ion-col/ion-card/ion-button[16]/span")));
            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("app-home")));
            var ionAppElement = element.FindElement(By.TagName("ion-app"));
            var mainElement = ionAppElement.FindElement(By.Id("main"));
            var ionContentElement = mainElement.FindElement(By.TagName("ion-content"));
            var appExtratoElement = ionContentElement.FindElement(By.TagName("app-extrato"));
            var ionContent2Element = appExtratoElement.FindElement(By.TagName("ion-content"));
            var formElement = ionContent2Element.FindElement(By.TagName("form"));
            var ionGridElement = formElement.FindElement(By.TagName("ion-grid"));
            var ionRowsElements = ionGridElement.FindElements(By.TagName("ion-row"));
            var ionColElement = ionRowsElements[1].FindElement(By.TagName("ion-col"));
            var ionCardElement = ionColElement.FindElement(By.TagName("ion-card"));
            _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("ion-button")));
            var ionButtonElement = ionCardElement.FindElements(By.TagName("ion-button"));

            Thread.Sleep(500);
            ionButtonElement[18].Click();

            return Task.CompletedTask;
        }

        public Task DownMenuPageScrollAfterOpenForm()
        {

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"extratoonline\"]/ion-row[2]/ion-col/ion-card/ion-button[16]/span")));
            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("app-home")));
            var ionAppElement = element.FindElement(By.TagName("ion-app"));
            var mainElement = ionAppElement.FindElement(By.Id("main"));
            var ionContentElement = mainElement.FindElement(By.TagName("ion-content"));
            var appExtratoElement = ionContentElement.FindElement(By.TagName("app-extrato"));
            var ionContent2Element = appExtratoElement.FindElement(By.TagName("ion-content"));

            var shadowRoot = ionContent2Element.GetShadowRoot();
            var scrollElement = shadowRoot.FindElement(By.CssSelector("[part=\"scroll\"]"));

            ExecuteJsScript("arguments[0].scrollTop += 1000;", scrollElement);

            return Task.CompletedTask;
        }

        public Task PerformSearch(string cpf)
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"extratoonline\"]/ion-row[2]/ion-col/ion-card/ion-grid/ion-row[2]/ion-col/ion-card/ion-item/ion-input/input")));
            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("app-home")));
            var ionAppElement = element.FindElement(By.TagName("ion-app"));
            var mainElement = ionAppElement.FindElement(By.Id("main"));
            var ionContentElement = mainElement.FindElement(By.TagName("ion-content"));
            var appExtratoElement = ionContentElement.FindElement(By.TagName("app-extrato"));
            var ionContent2Element = appExtratoElement.FindElement(By.TagName("ion-content"));
            var formElement = ionContent2Element.FindElement(By.TagName("form"));
            var ionGridElement = formElement.FindElement(By.TagName("ion-grid"));
            var ionRowsElements = ionGridElement.FindElements(By.TagName("ion-row"));
            var ionColElement = ionRowsElements[1].FindElement(By.TagName("ion-col"));
            var ionCardElement = ionColElement.FindElement(By.TagName("ion-card"));

            _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("ion-grid")));
            var ionGrid2Element = ionCardElement.FindElement(By.TagName("ion-grid"));
            var ionRows2Elements = ionGrid2Element.FindElements(By.TagName("ion-row"));
            var ionCard2Element = ionRows2Elements[1].FindElement(By.TagName("ion-card"));
            var ionItemElement = ionCard2Element.FindElement(By.TagName("ion-item"));
            var ionInputElement = ionItemElement.FindElement(By.TagName("ion-input"));
            var inputElement = ionInputElement.FindElement(By.TagName("input"));

            inputElement.Clear();
            Thread.Sleep(500);
            inputElement.SendKeys(cpf);

            var ionButtonElement = ionCard2Element.FindElement(By.TagName("ion-button"));
            var shadowRoot = ionButtonElement.GetShadowRoot();
            var buttonElement = shadowRoot.FindElement(By.CssSelector("[part=\"native\"]"));

            Actions actions = new Actions(_driver);

            Thread.Sleep(500);
            actions.MoveToElement(buttonElement);

            Thread.Sleep(500);
            actions.Click().Perform();

            return Task.CompletedTask;
        }

        public Task DownMenuPageScrollAfterSearch()
        {

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"extratoonline\"]/ion-row[2]/ion-col/ion-card/ion-button[1]")));
            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("app-home")));
            var ionAppElement = element.FindElement(By.TagName("ion-app"));
            var mainElement = ionAppElement.FindElement(By.Id("main"));
            var ionContentElement = mainElement.FindElement(By.TagName("ion-content"));
            var appExtratoElement = ionContentElement.FindElement(By.TagName("app-extrato"));
            var ionContent2Element = appExtratoElement.FindElement(By.TagName("ion-content"));

            var shadowRoot = ionContent2Element.GetShadowRoot();
            var scrollElement = shadowRoot.FindElement(By.CssSelector("[part=\"scroll\"]"));

            Thread.Sleep(500);
            ExecuteJsScript("arguments[0].scrollTop += 1000;", scrollElement);

            return Task.CompletedTask;
        }

        public Task<List<string>> CaptureResults()
        {
            var results = new List<string>();
            var ionItems = new List<IWebElement>();
            var ionLabels = new List<IWebElement>();

            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"extratoonline\"]/ion-row[2]/ion-col/ion-card/ion-grid/ion-row[2]/ion-col/ion-card/ion-card-header/ion-card-title")));
            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("app-home")));
            var ionAppElement = element.FindElement(By.TagName("ion-app"));
            var mainElement = ionAppElement.FindElement(By.Id("main"));
            var ionContentElement = mainElement.FindElement(By.TagName("ion-content"));
            var appExtratoElement = ionContentElement.FindElement(By.TagName("app-extrato"));
            var ionContent2Element = appExtratoElement.FindElement(By.TagName("ion-content"));
            var formElement = ionContent2Element.FindElement(By.TagName("form"));
            var ionGridElement = formElement.FindElement(By.TagName("ion-grid"));
            var ionRowsElements = ionGridElement.FindElements(By.TagName("ion-row"));
            var ionColElement = ionRowsElements[1].FindElement(By.TagName("ion-col"));
            var ionCardElement = ionColElement.FindElement(By.TagName("ion-card"));
            var ionGrid2Element = ionCardElement.FindElement(By.TagName("ion-grid"));
            var ionRows2Elements = ionGrid2Element.FindElements(By.TagName("ion-row"));
            var ionCol2Element = ionRows2Elements[1].FindElement(By.TagName("ion-col"));
            var ionCard2Element = ionCol2Element.FindElement(By.TagName("ion-card"));
            var ionItemsElements = ionCard2Element.FindElements(By.TagName("ion-item"));


            foreach (var item in ionItemsElements)
            {
                ionLabels.Add(item.FindElement(By.TagName("ion-label")));
            }

            foreach (var label in ionLabels)
            {
                results.Add(label.Text);
            }

            return Task.FromResult(results);
        }

        public Task BackToForm()
        {
            _wait.Until(ExpectedConditions.ElementIsVisible(By.XPath("//*[@id=\"extratoonline\"]/ion-row[2]/ion-col/ion-card/ion-grid/ion-row[2]/ion-col/ion-card/ion-card-header/ion-card-title")));
            var element = _wait.Until(ExpectedConditions.ElementIsVisible(By.TagName("app-home")));
            var ionAppElement = element.FindElement(By.TagName("ion-app"));
            var mainElement = ionAppElement.FindElement(By.Id("main"));
            var ionContentElement = mainElement.FindElement(By.TagName("ion-content"));
            var appExtratoElement = ionContentElement.FindElement(By.TagName("app-extrato"));
            var ionContent2Element = appExtratoElement.FindElement(By.TagName("ion-content"));
            var formElement = ionContent2Element.FindElement(By.TagName("form"));
            var ionGridElement = formElement.FindElement(By.TagName("ion-grid"));
            var ionRowsElements = ionGridElement.FindElements(By.TagName("ion-row"));
            var ionColElement = ionRowsElements[1].FindElement(By.TagName("ion-col"));
            var ionCardElement = ionColElement.FindElement(By.TagName("ion-card"));
            var ionGrid2Element = ionCardElement.FindElement(By.TagName("ion-grid"));
            var ionRows2Elements = ionGrid2Element.FindElements(By.TagName("ion-row"));
            var ionColElements = ionRows2Elements[0].FindElements(By.TagName("ion-col"));
            var ionButtonElement = ionColElements[0].FindElement(By.TagName("ion-button"));

            Thread.Sleep(500);
            ionButtonElement.Click();

            return Task.CompletedTask;
        }

        public Task CloseBrowser()
        {
            _driver.Quit();

            return Task.CompletedTask;
        }

        private Task ExecuteJsScript(string script, IWebElement element = null)
        {
            if (element == null)
                ((IJavaScriptExecutor)_driver).ExecuteScript(script);
            else
                ((IJavaScriptExecutor)_driver).ExecuteScript(script, element);

            return Task.CompletedTask;
        }
    }
}
