using POC_WebCrawler.Application.CQRS.Inputs;

namespace POC_WebCrawler.Web.StartupConfig
{
    public static class RegisterPackageConfig
    {
        public static IServiceCollection ResolvePackages(this IServiceCollection services)
        {
            services.AddControllers();
            services.AddEndpointsApiExplorer();
            services.AddSwaggerGen();
            services.AddMediatR(m => m.RegisterServicesFromAssembly(typeof(GetRegisterNumberByCPFQuery).Assembly));

            return services;
        }
    }
}
