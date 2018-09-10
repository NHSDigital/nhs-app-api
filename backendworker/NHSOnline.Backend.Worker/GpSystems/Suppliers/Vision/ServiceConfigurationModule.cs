using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;
        private readonly ILogger<ServiceConfigurationModule> _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.TryParse(configuration.GetOrWarn("GP_PROVIDER_ENABLED_VISION", _logger), out bool enabled) && enabled)
            {
                var configValue = configuration.ConfigurationSettings().GetOrWarn("DefaultHttpTimeoutSeconds",
                _logger);
                var timeout = int.Parse(configValue, CultureInfo.InvariantCulture);

                services.AddSingleton<IGpSystem, VisionGpSystem>();
                services.AddSingleton<IVisionConfig, VisionConfig>();
                services.AddSingleton<IVisionClient, VisionClient>();

                services.AddTransient<VisionTokenValidationService>();

                var certificateService = services.BuildServiceProvider().GetRequiredService<ICertificateService>();

                services.AddHttpClient<VisionHttpClient>(client =>
                    {
                        client.Timeout = TimeSpan.FromSeconds(timeout);
                    })
                    .ConfigurePrimaryHttpMessageHandler(() =>
                    {
                        return new VisionHttpClientHandler(configuration, _loggerFactory.CreateLogger<VisionHttpClientHandler>(), certificateService)
                            .ConfigureForwardProxy(configuration);
                    });

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