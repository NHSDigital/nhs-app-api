using System;
using NHSOnline.Backend.Support.Configuration;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Devices
{
    public class DeviceConfigurationSettings : IValidatable
    {
        public string NhsLoginLoggedInPaths { get; set; }

        public string MinimumSupportedAndroidVersion { get; set; }

        public string MinimumSupportediOSVersion { get; set; }

        public Uri FidoServerUrl { get; set; }

        public Uri WebAppBaseUrl { get; set; }

        public DeviceConfigurationSettings(string nhsLoginLoggedInPaths,
            string minimumSupportedAndroidVersion, string minimumSupportediOSVersion,
            Uri fidoServerUrl, Uri webAppBaseUrl)
        {
            MinimumSupportedAndroidVersion = minimumSupportedAndroidVersion;
            MinimumSupportediOSVersion = minimumSupportediOSVersion;
            NhsLoginLoggedInPaths = nhsLoginLoggedInPaths;
            FidoServerUrl = fidoServerUrl;
            WebAppBaseUrl = webAppBaseUrl;
        }

        public void Validate()
        {
            if (string.IsNullOrWhiteSpace(NhsLoginLoggedInPaths))
            {
                throw new ConfigurationNotFoundException(nameof(NhsLoginLoggedInPaths));
            }

            if (string.IsNullOrWhiteSpace(MinimumSupportedAndroidVersion))
            {
                throw new ConfigurationNotFoundException(nameof(MinimumSupportedAndroidVersion));
            }

            if(string.IsNullOrWhiteSpace(MinimumSupportediOSVersion))
            {
                throw new ConfigurationNotFoundException(nameof(MinimumSupportediOSVersion));
            }

            if (FidoServerUrl == null)
            {
                throw new ConfigurationNotFoundException(nameof(FidoServerUrl));
            }

            if (WebAppBaseUrl == null)
            {
                throw new ConfigurationNotFoundException(nameof(WebAppBaseUrl));
            }
        }
    }
}