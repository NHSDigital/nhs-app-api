using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {        
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Tpp;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<TppHttpClientHandler>();
            services.AddTransient<TppHttpRequestIdentifier>();

            services.AddSingleton<IGpSystem, TppGpSystem>();
            services.AddSingleton<ITppClient, TppClient>();
            services.AddSingleton<ITppConfig, TppConfig>();

            services.AddHttpClient<TppHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<TppHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<TppHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<TppHttpRequestIdentifier>>();

            services.AddTransient<TppTokenValidationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}