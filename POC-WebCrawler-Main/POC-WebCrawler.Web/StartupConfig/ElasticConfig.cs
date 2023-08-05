using Nest;

namespace POC_WebCrawler.Web.StartupConfig
{
    public static class ElasticConfig
    {
        public static IServiceCollection ResolveElasticConfiguration(this IServiceCollection services, IConfiguration configuration)
        {

            var settings = new ConnectionSettings(
                    new Uri(configuration["ElasticSettings:Domain"])
                );

            var elasticClient = new ElasticClient(settings);
            services.AddSingleton<IElasticClient>(elasticClient);

            return services;
        }
    }
}
