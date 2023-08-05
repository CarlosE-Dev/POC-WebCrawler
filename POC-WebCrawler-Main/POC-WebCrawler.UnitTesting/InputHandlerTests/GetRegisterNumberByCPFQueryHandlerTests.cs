using Moq;
using POC_WebCrawler.Application.CQRS.Inputs;
using POC_WebCrawler.Application.CQRS.Outputs;
using POC_WebCrawler.Domain.Entities;
using POC_WebCrawler.Domain.Interfaces;
using POC_WebCrawler.Shared.Utils;
using Xunit;

namespace POC_WebCrawler.UnitTesting.InputHandlerTests
{
    public class GetRegisterNumberByCPFQueryHandlerTests
    {
        private readonly GetRegisterNumberByCPFQueryHandler _handler;
        private readonly Mock<ICustomerService> _customerServiceMock;
        public GetRegisterNumberByCPFQueryHandlerTests()
        {
            _customerServiceMock = new Mock<ICustomerService>();
            _handler = new GetRegisterNumberByCPFQueryHandler(_customerServiceMock.Object);
        }

        [Fact]
        public async Task GetRegisterNumberByCPFQueryHandler_Should_Return_Exception_When_No_Results_Found()
        {
            //Arrange
            var cpfMock = GenerateString.GetCpfNumberMock();
            var query = new GetRegisterNumberByCPFQuery(cpfMock);
            _customerServiceMock.Setup(x => x.SearchCustomerDataByCpf(It.IsAny<string>())).ReturnsAsync(new List<Customer>());

            //Act
            var error = await Record.ExceptionAsync(async () =>
                await _handler.Handle(query, CancellationToken.None)
            );

            //Assert
            Assert.NotNull(error);
            Assert.IsType<Exception>(error);
        }

        [Fact]
        public async Task GetRegisterNumberByCPFQueryHandler_Should_Return_CustomerRegisterNumberResponse_When_Valid_Request()
        {
            //Arrange
            var cpfMock = GenerateString.GetCpfNumberMock();
            var regNumberMock = "123456789";
            var query = new GetRegisterNumberByCPFQuery(cpfMock);
            var customer = new Customer(cpfMock, new List<string> { regNumberMock });
            var list = new List<Customer> { customer };

            _customerServiceMock.Setup(x => x.SearchCustomerDataByCpf(It.IsAny<string>())).ReturnsAsync(list);

            //Act
            var response = await _handler.Handle(query, CancellationToken.None);

            //Assert
            Assert.NotNull(response);
            Assert.IsType<CustomerRegisterNumberResponse>(response);
            Assert.Equal(response.RegisterNumber.First(), regNumberMock);
        }
    }
}
