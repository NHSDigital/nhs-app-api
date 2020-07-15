using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
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
        private const char Separator = '|';

        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "ValidateAndLog covers this")]
        public ConfigurationService(
            ILogger<ConfigurationService> logger,
            IOptions<KnownServices> knownServicesOptions,
            DeviceConfigurationSettings settings)
        {
            ValidateAndLog.Using(logger)
                .KnownServicesConfigIsValid(knownServicesOptions)
                .IsValid();

            ValidateAndLog.Using(logger)
                .IsNotNull(settings, nameof(settings), ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNullOrWhitespace(settings?.NhsLoginLoggedInPaths, nameof(settings.NhsLoginLoggedInPaths),
                    ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNull(settings?.FidoServerUrl, nameof(settings.FidoServerUrl),
                    ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNullOrWhitespace(settings?.MinimumSupportediOSVersion, nameof(settings.MinimumSupportediOSVersion),
                    ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNullOrWhitespace(settings?.MinimumSupportedAndroidVersion,
                    nameof(settings.MinimumSupportedAndroidVersion), ValidateAndLog.ValidationOptions.ThrowError)
                .IsNotNull(settings?.WebAppBaseUrl, nameof(settings.WebAppBaseUrl),
                    ValidateAndLog.ValidationOptions.ThrowError)
                .IsValid();

            var nhsLoginLoggedInPaths = settings.NhsLoginLoggedInPaths
                .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            var knownServices = knownServicesOptions.Value.Services;

            knownServices.Where(s => string.Equals(s.Url.Host, "WebAppBaseUrl", StringComparison.OrdinalIgnoreCase))
                .ForEach(s => s.Url = settings.WebAppBaseUrl);

            _result = new GetConfigurationResultV2.Success(
                nhsLoginLoggedInPaths,
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