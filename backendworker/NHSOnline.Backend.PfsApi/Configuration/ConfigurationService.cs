using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using NHSOnline.Backend.PfsApi.Areas.Configuration;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Devices;

namespace NHSOnline.Backend.PfsApi.Configuration
{
    internal sealed class ConfigurationService: IConfigurationService
    {
        private readonly GetConfigurationResultV2 _result;
        private const char Separator = '|';

        [SuppressMessage("ReSharper", "PossibleNullReferenceException", Justification = "ValidateAndLog covers this")]
        public ConfigurationService(
            KnownServices knownServices,
            DeviceConfigurationSettings settings)
        {
            knownServices.Validate();
            settings.Validate();

            var nhsLoginLoggedInPaths = settings.NhsLoginLoggedInPaths
                .Split(Separator, StringSplitOptions.RemoveEmptyEntries)
                .Select(p => p.Trim())
                .ToList();

            knownServices.FixUpWebAppBaseUrl(settings.WebAppBaseUrl);

            _result = new GetConfigurationResultV2(
                nhsLoginLoggedInPaths,
                settings.FidoServerUrl,
                settings.MinimumSupportedAndroidVersion,
                settings.MinimumSupportediOSVersion,
                knownServices.Services.Values.ToList());
        }

        [SuppressMessage("Microsoft.Design", "CA1024", Justification = "Intentional; do not wish consumers to treat this as a property")]
        public GetConfigurationResultV2 GetConfiguration()
        {
            return _result;
        }
    }
}