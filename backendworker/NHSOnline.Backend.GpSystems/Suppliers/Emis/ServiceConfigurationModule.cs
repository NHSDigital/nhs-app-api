using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }
        
        protected override Supplier Supplier => Supplier.Emis;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<EmisHttpClientHandler>();
            services.AddTransient<EmisHttpRequestIdentifier>();

            services.AddSingleton<IGpSystem, EmisGpSystem>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();

            services.AddHttpClient<EmisHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<EmisHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<EmisHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<EmisHttpRequestIdentifier>>();

            services.AddTransient<IEmisEnumMapper, EmisEnumMapper>();

            services.AddTransient<EmisTokenValidationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}