using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Http;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class ServiceConfigurationModule : Support.DependencyInjection.SupplierServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ServiceConfigurationModule> _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory) : base(loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = _loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        protected override Supplier Supplier => Supplier.Vision;
        
        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {

            if (bool.TryParse(configuration.GetOrWarn("GP_PROVIDER_ENABLED_VISION", _logger), out bool enabled) && enabled)
            {
                services.AddSingleton<IGpSystem, VisionGpSystem>();
                services.AddSingleton<IVisionConfig, VisionConfig>();
                services.AddSingleton<IVisionClient, VisionClient>();

                services.AddTransient<VisionTokenValidationService>();
                
                services.AddTransient<VisionHttpRequestIdentifier>();
                services.AddSingleton<VisionHttpClientHandler>();

                services.AddHttpClient<VisionHttpClient>()
                    .ConfigurePrimaryHttpMessageHandler<VisionHttpClientHandler>()
                    .AddHttpMessageHandler<HttpTimeoutHandler<VisionHttpRequestIdentifier>>()
                    .AddHttpMessageHandler<HttpRequestIdentificationHandler<VisionHttpRequestIdentifier>>();

                _logger.LogDebug("Vision GP Service was successfully configured");
            }
            else
            {
                _logger.LogDebug("Vision GP Service was not configured");
            }

            base.ConfigureServices(services, configuration);
        }
    }
}