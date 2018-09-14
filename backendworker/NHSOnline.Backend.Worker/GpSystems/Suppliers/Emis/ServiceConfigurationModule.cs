using System;
using System.Globalization;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Linkage;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Session;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILogger<ServiceConfigurationModule> _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            if (bool.TryParse(configuration.GetOrWarn("GP_PROVIDER_ENABLED_EMIS", _logger), out bool enabled) && enabled)
            {
                var defaultHttpTimeoutSeconds = configuration.ConfigurationSettings().GetOrWarn("DefaultHttpTimeoutSeconds",
                _logger);

                services.AddHttpClient<EmisHttpClient>(client =>
                {
                    client.Timeout =
                        TimeSpan.FromSeconds(int.Parse(defaultHttpTimeoutSeconds, CultureInfo.InvariantCulture));
                }).ConfigurePrimaryHttpMessageHandler(() =>
                    new HttpClientHandler().ConfigureForwardProxy(configuration));

                services.AddSingleton<IGpSystem, EmisGpSystem>();
                services.AddSingleton<IEmisClient, EmisClient>();
                services.AddSingleton<IEmisConfig, EmisConfig>();
                services.AddSingleton<IEmisSessionService, EmisSessionService>();
                services.AddSingleton<IRegistrationGuidKeyGenerator, EmisRegistrationGuidKeyGenerator>();
                services.AddSingleton<IRegistrationCacheService, RegistrationCacheService>();

                services.AddTransient<EmisTokenValidationService>();

                _logger.LogDebug("Emis GP Service was successfully configured");
            }
            else
            {
                _logger.LogDebug("Emis GP Service was not configured");
            }

            base.ConfigureServices(services, configuration);

        }
    }
}