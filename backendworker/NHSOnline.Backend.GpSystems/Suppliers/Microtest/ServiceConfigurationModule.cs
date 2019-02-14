using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.DependencyInjection;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class ServiceConfigurationModule : SupplierServiceConfigurationModule
    {
        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
        }

        protected override Supplier Supplier => Supplier.Microtest;

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<MicrotestHttpClientHandler>();

            services.AddHttpClient<MicrotestHttpClient>()
                .ConfigurePrimaryHttpMessageHandler<MicrotestHttpClientHandler>()
                .AddHttpMessageHandler<HttpTimeoutHandler<MicrotestHttpRequestIdentifier>>()
                .AddHttpMessageHandler<HttpRequestIdentificationHandler<MicrotestHttpRequestIdentifier>>();

            services.AddSingleton<IGpSystem, MicrotestGpSystem>();
            services.AddSingleton<IMicrotestClient, MicrotestClient>();
            services.AddSingleton<IMicrotestConfig, MicrotestConfig>();
            services.AddTransient<MicrotestTokenValidationService>();

            base.ConfigureServices(services, configuration);
        }
    }
}