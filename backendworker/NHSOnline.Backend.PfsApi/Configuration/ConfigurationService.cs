using System;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.PfsApi.Areas.Configuration;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Devices;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Configuration
{
    public class ConfigurationService: IConfigurationService
    {
        private readonly GetConfigurationResultV2 _result;

        public ConfigurationService(
            ILogger<ConfigurationService> logger,
            IOptions<KnownServices> knownServicesOptions,
            DeviceConfigurationSettings settings)
        {
            new ValidateAndLog(logger)
                .IsNotNull(knownServicesOptions, nameof(knownServicesOptions), ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNull(knownServicesOptions?.Value, nameof(knownServicesOptions.Value), ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();

            new ValidateAndLog(logger)
                .IsNotNull(settings, nameof(settings), ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNull(settings?.FidoServerUrl, nameof(settings.FidoServerUrl),
                    ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNullOrWhitespace(settings?.MinimumSupportediOSVersion, nameof(settings.MinimumSupportediOSVersion),
                    ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNullOrWhitespace(settings?.MinimumSupportedAndroidVersion,
                    nameof(settings.MinimumSupportedAndroidVersion), ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNull(settings?.WebAppBaseUrl, nameof(settings.WebAppBaseUrl),
                    ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();

            var knownServices = knownServicesOptions.Value.Services;

            foreach (var service in knownServices)
            {
                if (string.Equals(service.Url.Host, "WebAppBaseUrl", StringComparison.OrdinalIgnoreCase))
                {
                    service.Url = settings.WebAppBaseUrl;
                }
            }
            
            _result = new GetConfigurationResultV2.Success(
                settings.FidoServerUrl,
                settings.MinimumSupportedAndroidVersion,
                settings.MinimumSupportediOSVersion,
                knownServices
            );
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Intentional; do not wish consumers to treat this as a property")]
        public GetConfigurationResultV2 GetConfiguration()
        {
            return _result;
        }
    }
}