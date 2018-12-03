using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ServiceConfigurationModule> _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }
        
        protected override Supplier Supplier => Supplier.Emis;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            services.AddSingleton<EmisHttpClientHandler>();
            services.AddTransient<EmisHttpRequestIdentifier>();

            services.AddSingleton<IGpSystem, EmisGpSystem>();
            services.AddSingleton<IEmisClient, EmisClient>();
            services.AddSingleton<IEmisConfig, EmisConfig>();
            services.AddSingleton<IEmisSessionService, EmisSessionService>();

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