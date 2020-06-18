using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Devices
{
    public sealed class ServiceConfigurationModule : Support.DependencyInjection.ServiceConfigurationModule
    {
        private readonly ILogger _logger;

        public ServiceConfigurationModule(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<ServiceConfigurationModule>();
        }

        public override void ConfigureServices(IServiceCollection services, IConfiguration configuration)
        {
            var deviceConfigurationSettings = CreateAndValidateDeviceEnvironmentVariables(configuration);
            services.AddSingleton(deviceConfigurationSettings);

            services.AddSingleton<ISupportedDeviceService, SupportedDeviceService>();

            base.ConfigureServices(services, configuration);
        }

        private DeviceConfigurationSettings CreateAndValidateDeviceEnvironmentVariables(IConfiguration configuration)
        {
            var webAppBaseUrl = new Uri(configuration.GetOrWarn("NHS_WEB_APP_BASE_URL", _logger), UriKind.Absolute);

            var nhsLoginLoggedInPaths = configuration["ConfigurationSettings:NhsLoginLoggedInPaths"];
            var minimumSupportedAndroidVersion = configuration["ConfigurationSettings:MinimumSupportedAndroidVersion"];
            var minimumSupportediOSVersion = configuration["ConfigurationSettings:MinimumSupportediOSVersion"];
            var fidoServerUrl = new Uri(configuration["ConfigurationSettings:FidoServerUrl"], UriKind.Absolute);

            var config = new DeviceConfigurationSettings(nhsLoginLoggedInPaths, minimumSupportedAndroidVersion,
                minimumSupportediOSVersion, fidoServerUrl, webAppBaseUrl);
            config.Validate();

            return config;
        }

    }
}