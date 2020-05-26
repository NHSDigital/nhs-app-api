using System;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.DependencyInjection
{
    public sealed class SupplierStartup
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<SupplierStartup> _logger;
        private readonly IGpSystemRegistrationService _gpSystemRegistrationService;
        public SupplierStartup(IConfiguration configuration, ILoggerFactory loggerFactory, IGpSystemRegistrationService gpSystemRegistrationService)
        {
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
            _logger = loggerFactory.CreateLogger<SupplierStartup>();
            _gpSystemRegistrationService = gpSystemRegistrationService;

        }

        public void ConfigureServices(IServiceCollection services)
        {
            var gpSupplierConfig = new Dictionary<string, Action<EnableGpSupplierConfiguration, bool>>
            {
                { "GP_PROVIDER_ENABLED_EMIS", (config, enabled) => config.EnableEmis = enabled },
                { "GP_PROVIDER_ENABLED_VISION", (config, enabled) => config.EnableVision = enabled },
                { "GP_PROVIDER_ENABLED_TPP", (config, enabled) => config.EnableTpp = enabled },
                { "GP_PROVIDER_ENABLED_MICROTEST", (config, enabled) => config.EnableMicrotest = enabled },
                { "GP_PROVIDER_ENABLED_FAKE", (config, enabled) => config.EnableFake = enabled },
            };

            _logger.LogInformation("Registering GP services");

            var enableGpSupplierConfig = new EnableGpSupplierConfiguration();

            foreach (var config in gpSupplierConfig)
            {
                var configValue = _configuration.GetOrWarn(config.Key, _logger);

                if (bool.TryParse(configValue, out var enableGpSupplier))
                {
                    _logger.LogInformation($"GP supplier config { config.Key } value: { enableGpSupplier }");
                }
                else
                {
                    _logger.LogWarning($"GP supplier config { config.Key } value not a valid boolean: { configValue }");
                }

                config.Value(enableGpSupplierConfig, enableGpSupplier);
            }

            _gpSystemRegistrationService.RegisterPfsServices(services, enableGpSupplierConfig);
        }
    }
}
