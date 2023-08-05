using Microsoft.Extensions.Configuration;
using Moq;
using POC_WebCrawler.Application.Services;
using POC_WebCrawler.Domain.Entities;
using POC_WebCrawler.Domain.Interfaces;
using POC_WebCrawler.Shared.Utils;
using Xunit;

namespace POC_WebCrawler.UnitTesting.ServiceTests
{
    public class DataCrawlerServiceTests
    {
        private readonly DataCrawlerService _service;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<ICustomerService> _customerServiceMock;
        private readonly Mock<IWebDriverCrawler> _webDriverCrawlerMock;
        public DataCrawlerServiceTests()
        {
            _configurationMock = new Mock<IConfiguration>();
            _customerServiceMock = new Mock<ICustomerService>();
            _webDriverCrawlerMock = new Mock<IWebDriverCrawler>();
            _service = new DataCrawlerService(
                _configurationMock.Object,
                _customerServiceMock.Object,
                _webDriverCrawlerMock.Object
            );
        }

        [Fact]
        public async Task Execute_Should_Return_Exception_When_Cpf_List_Not_Found_In_Queue()
        {
            // Arrange
            _customerServiceMock.Setup(x => x.ConsumeListFromQueue()).ReturnsAsync(new List<string>());

            // Act
            var error = await Record.ExceptionAsync(async () =>
                await _service.Execute()
            );

            // Assert
            Assert.NotNull(error);
            Assert.IsType<Exception>(error);
            Assert.Equal("CPF list not found.", error.Message);
        }

        [Fact]
        public async Task Execute_Should_Return_0_When_Cpf_List_Already_Cached()
        {
            // Arrange
            var customerCpf = GenerateString.GetCpfNumberMock();
            var cpfList = new List<string> { customerCpf };
            var customerList = new List<Customer> {
                new Customer(
                    customerCpf,
                    new List<string> {
                        GenerateString.RandomStringNumeric(10)
                    }
                )
            };

            _customerServiceMock.Setup(x => x.ConsumeListFromQueue()).ReturnsAsync(cpfList);
            _customerServiceMock.Setup(x => x.GetCachedCustomersList()).ReturnsAsync(customerList);

            // Act
            var result = await _service.Execute();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_Should_Return_0_When_No_Results_In_Search()
        {
            // Arrange
            var customerCpf = GenerateString.GetCpfNumberMock();
            var cpfList = new List<string> { customerCpf };
            var emptyCustomerList = new List<Customer>();
            var emptyRegisterNumberList = new List<string>();

            await MockWebDriverVoidMethods();
            _customerServiceMock.Setup(x => x.ConsumeListFromQueue()).ReturnsAsync(cpfList);
            _customerServiceMock.Setup(x => x.GetCachedCustomersList()).ReturnsAsync(emptyCustomerList);
            _webDriverCrawlerMock.Setup(x => x.CaptureResults()).Returns(Task.FromResult(emptyRegisterNumberList));
            _customerServiceMock.Setup(x => x.IndexCustomerData(It.IsAny<Customer>())).Returns(Task.CompletedTask);
            _customerServiceMock.Setup(x => x.StoreInCache(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Execute();

            // Assert
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task Execute_Should_Return_Count_Of_New_Registers_When_Successful()
        {
            // Arrange
            var customerCpf = GenerateString.GetCpfNumberMock();
            var cpfList = new List<string> { customerCpf };
            var emptyCustomerList = new List<Customer>();
            var registerNumberMock = GenerateString.RandomStringNumeric(10);
            var registerNumberList = new List<string> { registerNumberMock };

            await MockWebDriverVoidMethods();
            _customerServiceMock.Setup(x => x.ConsumeListFromQueue()).ReturnsAsync(cpfList);
            _customerServiceMock.Setup(x => x.GetCachedCustomersList()).ReturnsAsync(emptyCustomerList);
            _webDriverCrawlerMock.Setup(x => x.CaptureResults()).Returns(Task.FromResult(registerNumberList));
            _customerServiceMock.Setup(x => x.IndexCustomerData(It.IsAny<Customer>())).Returns(Task.CompletedTask);
            _customerServiceMock.Setup(x => x.StoreInCache(It.IsAny<string>())).Returns(Task.CompletedTask);

            // Act
            var result = await _service.Execute();

            // Assert
            Assert.Equal(1, result);
        }

        private Task MockWebDriverVoidMethods()
        {
            _webDriverCrawlerMock.Setup(x => x.EnterWebSite(It.IsAny<string>())).Returns(Task.CompletedTask);
            _webDriverCrawlerMock.Setup(x => x.Authenticate(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);
            _webDriverCrawlerMock.Setup(x => x.ClosePopupAfterLogin()).Returns(Task.CompletedTask);
            _webDriverCrawlerMock.Setup(x => x.GoToSearchForm()).Returns(Task.CompletedTask);
            _webDriverCrawlerMock.Setup(x => x.PerformSearch(It.IsAny<string>())).Returns(Task.CompletedTask);
            _webDriverCrawlerMock.Setup(x => x.DownMenuPageScrollAfterOpenForm()).Returns(Task.CompletedTask);
            _webDriverCrawlerMock.Setup(x => x.DownMenuPageScrollAfterLogin()).Returns(Task.CompletedTask);
            _webDriverCrawlerMock.Setup(x => x.DownMenuPageScrollAfterSearch()).Returns(Task.CompletedTask);

            return Task.CompletedTask;
        }
    }
}
