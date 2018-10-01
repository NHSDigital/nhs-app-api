using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILoggerFactory _loggerFactory;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _loggerFactory = loggerFactory;
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var logger = _loggerFactory.CreateLogger<ServiceConfigurationModule>();

            if (bool.TryParse(configuration.GetOrWarn("GP_PROVIDER_ENABLED_VISION", logger), out bool enabled) && enabled)
            {
                var configValue = configuration.ConfigurationSettings().GetOrWarn("DefaultHttpTimeoutSeconds",
                logger);

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
                            new VisionHttpClientHandler(configuration,
                                _loggerFactory.CreateLogger<VisionHttpClientHandler>(),
                                certificateService)
                        );

                logger.LogDebug("Vision GP Service was successfully configured");
            }
            else
            {
                logger.LogDebug("Vision GP Service was not configured");
            }

            base.ConfigureServices(services, configuration);
        }
    }
}