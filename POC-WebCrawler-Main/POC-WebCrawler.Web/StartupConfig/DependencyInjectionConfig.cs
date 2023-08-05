using OpenQA.Selenium.Chrome;
using OpenQA.Selenium;
using POC_WebCrawler.Application.Services;
using POC_WebCrawler.Domain.Interfaces;
using POC_WebCrawler.Infra.ElasticSearch;
using POC_WebCrawler.Infra.RabbitMQ;
using POC_WebCrawler.Infra.Redis;
using POC_WebCrawler.Infra.WebDriver;
using RabbitMQ.Client;

namespace POC_WebCrawler.Web.StartupConfig
{
    public static class DependencyInjectionConfig
    {
        public static IServiceCollection ResolveDependencies(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddScoped<IDataCrawlerService, DataCrawlerService>();
            services.AddScoped<ICustomerService, CustomerService>();
            services.AddScoped<IElasticSearchApiRepository, ElasticSearchApiRepository>();
            services.AddScoped<IRedisCacheRepository, RedisCacheRepository>();
            services.AddScoped<IRabbitMQProducer, RabbitMQProducer>();
            services.AddScoped<IRabbitMQConsumer, RabbitMQConsumer>();
            services.AddScoped<IWebDriverCrawler, WebDriverCrawler>();
            services.AddSingleton<IWebDriver>(provider => {
                ChromeOptions options = new ChromeOptions();
                options.AddArgument("--headless");
                options.AddArgument("--window-size=1280,720");
                options.AddArgument("--disable-extensions");
                options.AddArgument("--no-sandbox");
                options.AddArgument("--disable-notifications");
                options.AddArgument("--disable-infobars");
                return new ChromeDriver(options);
            });

            services.AddSingleton<ConnectionFactory>(sp =>
            {
                return new ConnectionFactory
                {
                    HostName = configuration["RabbitMQSettings:Host"],
                    UserName = configuration["RabbitMQSettings:User"],
                    Password = configuration["RabbitMQSettings:Password"]
                };
            });

            services.AddScoped<IConnection>(sp =>
            {
                var factory = sp.GetRequiredService<ConnectionFactory>();
                return factory.CreateConnection();
            });

            services.AddScoped<IModel>(sp =>
            {
                var connection = sp.GetRequiredService<IConnection>();
                return connection.CreateModel();
            });

            return services;
        }
    }
}
