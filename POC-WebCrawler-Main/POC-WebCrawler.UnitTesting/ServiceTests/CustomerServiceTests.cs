using Microsoft.Extensions.Configuration;
using Moq;
using Nest;
using POC_WebCrawler.Application.Services;
using POC_WebCrawler.Domain.Entities;
using POC_WebCrawler.Domain.Interfaces;
using POC_WebCrawler.Shared.Utils;
using System.Text.Json;
using Xunit;

namespace POC_WebCrawler.UnitTesting.ServiceTests
{
    public class CustomerServiceTests
    {
        private readonly Mock<IElasticSearchApiRepository> _elasticApiRepositoryMock;
        private readonly Mock<IConfiguration> _configurationMock;
        private readonly Mock<IRedisCacheRepository> _cacheRepositoryMock;
        private readonly Mock<IRabbitMQProducer> _producerMock;
        private readonly Mock<IRabbitMQConsumer> _consumerMock;
        private readonly CustomerService _service;
        public CustomerServiceTests()
        {
            _elasticApiRepositoryMock = new Mock<IElasticSearchApiRepository>();
            _configurationMock = new Mock<IConfiguration>();
            _cacheRepositoryMock = new Mock<IRedisCacheRepository>();
            _producerMock = new Mock<IRabbitMQProducer>();
            _consumerMock = new Mock<IRabbitMQConsumer>();
            _service = new CustomerService(
                _configurationMock.Object, 
                _elasticApiRepositoryMock.Object, 
                _cacheRepositoryMock.Object, 
                _producerMock.Object, 
                _consumerMock.Object
                );
        }

        [Fact]
        public async Task CreateCustomerIndex_Should_Return_Exception_When_Duplicate_Index()
        {
            //Arrange
            var indexMock = "index";
            _elasticApiRepositoryMock.Setup(x => x.IndexExists(It.IsAny<string>())).ReturnsAsync(true);

            //Act
            var error = await Record.ExceptionAsync(() =>
                _service.CreateCustomerIndex(indexMock)
            );

            //Assert
            Assert.NotNull(error.Message);
            Assert.IsType<Exception>(error);
        }

        [Fact]
        public async Task CreateCustomerIndex_Should_Return_Exception_When_Index_Invalid()
        {
            //Arrange
            var indexMock = "index";
            var response = new Mock<CreateIndexResponse>();
            response.Setup(x => x.IsValid).Returns(false);
            
            _elasticApiRepositoryMock.Setup(x => x.IndexExists(It.IsAny<string>())).ReturnsAsync(false);
            _elasticApiRepositoryMock.Setup(x => x.CreateIndex<Customer>(It.IsAny<string>())).Returns(Task.FromResult(response.Object));

            //Act
            var error = await Record.ExceptionAsync(() =>
                _service.CreateCustomerIndex(indexMock)
            );

            //Assert
            Assert.NotNull(error.Message);
            Assert.IsType<Exception>(error);
        }

        [Fact]
        public async Task CreateCustomerIndex_Should_Successful_When_Valid_Request()
        {
            //Arrange
            var indexMock = "index";
            var response = new Mock<CreateIndexResponse>();
            response.Setup(x => x.IsValid).Returns(true);
            
            _elasticApiRepositoryMock.Setup(x => x.IndexExists(It.IsAny<string>())).ReturnsAsync(false);
            _elasticApiRepositoryMock.Setup(x => x.CreateIndex<Customer>(It.IsAny<string>())).Returns(Task.FromResult(response.Object));

            //Act
            var error = await Record.ExceptionAsync(() =>
                _service.CreateCustomerIndex(indexMock)
            );

            //Assert
            Assert.Null(error);
        }

        [Fact]
        public async Task IndexCustomerData_Should_Not_Return_Exception_When_Duplicated_Data()
        {
            //Arrange
            var customerMock = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });

            _elasticApiRepositoryMock.Setup(x => x.IndexExists(It.IsAny<string>())).ReturnsAsync(true);
            _elasticApiRepositoryMock.Setup(x => x.DataExists<Customer>(customerMock.Cpf, It.IsAny<string>())).ReturnsAsync(true);

            //Act
            var error = await Record.ExceptionAsync(() =>
                _service.IndexCustomerData(customerMock)
            );

            //Assert
            Assert.Null(error);
        }

        [Fact]
        public async Task IndexCustomerData_Should_Return_Exception_When_Invalid_Request()
        {
            //Arrange
            var customerMock = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });
            var response = new Mock<IndexResponse>();
            response.Setup(x => x.IsValid).Returns(false);

            _elasticApiRepositoryMock.Setup(x => x.IndexExists(It.IsAny<string>())).ReturnsAsync(true);
            _elasticApiRepositoryMock.Setup(x => x.DataExists<Customer>(customerMock.Cpf, It.IsAny<string>())).ReturnsAsync(false);
            _elasticApiRepositoryMock.Setup(x => x.IndexData(It.IsAny<string>(), customerMock, customerMock.Cpf)).Returns(Task.FromResult(response.Object));

            //Act
            var error = await Record.ExceptionAsync(() =>
                _service.IndexCustomerData(customerMock)
            );

            //Assert
            Assert.NotNull(error);
            Assert.IsType<Exception>(error);
        }

        [Fact]
        public async Task IndexCustomerData_Should_Successful_When_Valid_Request()
        {
            //Arrange
            var customerMock = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });
            var response = new Mock<IndexResponse>();
            response.Setup(x => x.IsValid).Returns(true);

            _elasticApiRepositoryMock.Setup(x => x.IndexExists(It.IsAny<string>())).ReturnsAsync(true);
            _elasticApiRepositoryMock.Setup(x => x.DataExists<Customer>(customerMock.Cpf, It.IsAny<string>())).ReturnsAsync(false);
            _elasticApiRepositoryMock.Setup(x => x.IndexData(It.IsAny<string>(), customerMock, customerMock.Cpf)).Returns(Task.FromResult(response.Object));

            //Act
            var error = await Record.ExceptionAsync(() =>
                _service.IndexCustomerData(customerMock)
            );

            //Assert
            Assert.Null(error);
        }

        [Fact]
        public async Task SearchCustomerDataByCpf_Should_Successful_When_Valid_Request()
        {
            //Arrange
            var customerMock = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });
            var customerListMock = new List<Customer>() { customerMock };

            _elasticApiRepositoryMock.Setup(x => x.SearchDataByProperty<Customer>(It.IsAny<string>(), It.IsAny<string>(), customerMock.Cpf)).ReturnsAsync(customerListMock);

            //Act
            var error = await Record.ExceptionAsync(() =>
                _service.SearchCustomerDataByCpf(customerMock.Cpf)
            );

            //Assert
            Assert.Null(error);
        }

        [Fact]
        public async Task SearchCustomerDataByCpf_Should_Return_Exception_When_No_Results_Found()
        {
            //Arrange
            var cpfMock = GenerateString.GetCpfNumberMock();
            var emptyListMock = new List<Customer>();

            _elasticApiRepositoryMock.Setup(x => x.SearchDataByProperty<Customer>(It.IsAny<string>(), It.IsAny<string>(), cpfMock)).ReturnsAsync(emptyListMock);

            //Act
            var error = await Record.ExceptionAsync(() =>
                _service.SearchCustomerDataByCpf(cpfMock)
            );

            //Assert
            Assert.NotNull(error);
            Assert.IsType<Exception>(error);
        }

        [Fact]
        public async Task GetCachedCustomersList_Should_Return_Empty_List_When_No_Results_Found()
        {
            //Arrange
            var emptyListMock = new List<Customer>();

            _cacheRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(JsonSerializer.Serialize(emptyListMock));

            //Act
            var result = await _service.GetCachedCustomersList();

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Count == 0);
        }

        [Fact]
        public async Task GetCachedCustomersList_Should_Return_Customers_List_When_Results_Found()
        {
            //Arrange
            var customerMock = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });
            var customerListMock = new List<Customer>() { customerMock };

            _cacheRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(JsonSerializer.Serialize(customerListMock));

            //Act
            var result = await _service.GetCachedCustomersList();

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
        }

        [Fact]
        public async Task StoreInCache_Should_Store_List_In_Cache_When_No_Cached_Results()
        {
            //Arrange
            var emptyListMock = new List<Customer>();
            var customerMock = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });
            var customerListMock = new List<Customer>() { customerMock };

            _cacheRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(JsonSerializer.Serialize(emptyListMock));
            _cacheRepositoryMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            //Act
            await _service.StoreInCache(JsonSerializer.Serialize(customerListMock));

            //Assert
            _cacheRepositoryMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task StoreInCache_Should_Not_Store_In_Cache_When_Found_Same_Cached_Results()
        {
            //Arrange
            var customerMock = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });
            var customerListMock = new List<Customer>() { customerMock };

            _cacheRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(JsonSerializer.Serialize(customerListMock));
            _cacheRepositoryMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            //Act
            await _service.StoreInCache(JsonSerializer.Serialize(customerListMock));

            //Assert
            _cacheRepositoryMock.Verify(x => x.Clean(It.IsAny<string>()), Times.Never);
            _cacheRepositoryMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public async Task StoreInCache_Should_Replace_Cache_When_Found_Another_Cached_Results()
        {
            //Arrange
            var customerMock = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });
            var customerMock2 = new Customer(GenerateString.GetCpfNumberMock(), new List<string> { "12345678" });
            var customerListMock = new List<Customer>() { customerMock };
            var customerListMock2 = new List<Customer>() { customerMock2 };

            _cacheRepositoryMock.Setup(x => x.Get(It.IsAny<string>())).ReturnsAsync(JsonSerializer.Serialize(customerListMock));
            _cacheRepositoryMock.Setup(x => x.Set(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            //Act
            await _service.StoreInCache(JsonSerializer.Serialize(customerListMock2));

            //Assert
            _cacheRepositoryMock.Verify(x => x.Clean(It.IsAny<string>()), Times.Once);
            _cacheRepositoryMock.Verify(x => x.Set(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task CleanCustomerCache_Should_Successful_When_Valid_Request()
        {
            //Arrange
            _cacheRepositoryMock.Setup(x => x.Clean(It.IsAny<string>())).Returns(Task.CompletedTask);

            //Act
            await _service.CleanCustomerCache();

            //Assert
            _cacheRepositoryMock.Verify(x => x.Clean(It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task FillCustomerQueue_Should_Successful_When_Valid_Request()
        {
            //Arrange
            _producerMock.Setup(x => x.Send(It.IsAny<string>(), It.IsAny<string>())).Returns(Task.CompletedTask);

            //Act
            await _service.FillCustomerQueue();

            //Assert
            _producerMock.Verify(x => x.Send(It.IsAny<string>(), It.IsAny<string>()), Times.Once);
        }

        [Fact]
        public async Task ConsumeListFromQueue_Should_Return_CpfList_When_Valid_Request()
        {
            //Arrange
            var listMock = new List<string> { GenerateString.GetCpfNumberMock(), GenerateString.GetCpfNumberMock() };

            _consumerMock.Setup(x => x.Consume(It.IsAny<string>())).ReturnsAsync(JsonSerializer.Serialize(listMock));

            //Act
            var result = await _service.ConsumeListFromQueue();

            //Assert
            Assert.NotNull(result);
            Assert.True(result.Any());
            Assert.Equal(result, listMock);
        }

        [Fact]
        public async Task ConsumeListFromQueue_Should_Return_Empty_List_When_No_Results_Found()
        {
            //Arrange
            var listMock = new List<string>();

            _consumerMock.Setup(x => x.Consume(It.IsAny<string>())).ReturnsAsync(JsonSerializer.Serialize(listMock));

            //Act
            var result = await _service.ConsumeListFromQueue();

            //Assert
            Assert.NotNull(result);
            Assert.IsType<List<string>>(result);
            Assert.False(result.Any());
        }
    }
}
