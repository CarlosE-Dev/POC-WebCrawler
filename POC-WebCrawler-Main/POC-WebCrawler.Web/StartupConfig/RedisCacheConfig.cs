namespace POC_WebCrawler.Web.StartupConfig
{
    public static class RedisCacheConfig
    {
        public static IServiceCollection ResolveRedisCacheConfiguration(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = configuration["RedisCachingSettings:Host"];
                options.InstanceName = configuration["RedisCachingSettings:Instance"];
            });

            return services;
        }
    }
}
