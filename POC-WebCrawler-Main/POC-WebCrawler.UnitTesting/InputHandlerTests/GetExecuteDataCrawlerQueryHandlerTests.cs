using Moq;
using POC_WebCrawler.Application.CQRS.Inputs;
using POC_WebCrawler.Domain.Interfaces;
using Xunit;

namespace POC_WebCrawler.UnitTesting.InputHandlerTests
{
    public class GetExecuteDataCrawlerQueryHandlerTests
    {
        private readonly GetExecuteDataCrawlerQueryHandler _handler;
        private readonly Mock<IDataCrawlerService> _dataCrawlerServiceMock;
        public GetExecuteDataCrawlerQueryHandlerTests()
        {
            _dataCrawlerServiceMock = new Mock<IDataCrawlerService>();
            _handler = new GetExecuteDataCrawlerQueryHandler(_dataCrawlerServiceMock.Object);
        }

        [Fact]
        public async Task GetExecuteDataCrawlerQueryHandler_Should_Return_0_When_Valid_Request()
        {
            //Arrange
            var request = new GetExecuteDataCrawlerQuery();
            _dataCrawlerServiceMock.Setup(x => x.Execute()).ReturnsAsync(0);
            
            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            Assert.IsType<int>(result);
            Assert.Equal(0, result);
        }

        [Fact]
        public async Task GetExecuteDataCrawlerQueryHandler_Should_Return_The_Number_Of_Registers_When_Valid_Request()
        {
            //Arrange
            var request = new GetExecuteDataCrawlerQuery();
            _dataCrawlerServiceMock.Setup(x => x.Execute()).ReturnsAsync(5);

            //Act
            var result = await _handler.Handle(request, CancellationToken.None);

            //Assert
            Assert.IsType<int>(result);
            Assert.Equal(5, result);
        }
    }
}
