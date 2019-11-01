using System;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.PfsApi.Devices 
{
    public class DeviceConfigurationSettings : IValidatable
    {
        public string MinimumSupportedAndroidVersion { get; set; }
        public string MinimumSupportediOSVersion { get; set; }

        public Uri FidoServerUrl { get; set; }

        public string ThrottlingEnabled { get; set; }

        public DeviceConfigurationSettings() {}
        
        public DeviceConfigurationSettings(string minimumSupportedAndroidVersion, string minimumSupportediOSVersion, Uri fidoServerUrl, string throttlingEnabled) 
        {
            MinimumSupportedAndroidVersion = minimumSupportedAndroidVersion;
            MinimumSupportediOSVersion  = minimumSupportediOSVersion;
            FidoServerUrl = fidoServerUrl;
            ThrottlingEnabled = throttlingEnabled;
        }

        public void Validate()
        {
            if (MinimumSupportedAndroidVersion == null)
            {
                throw new ConfigurationNotFoundException(nameof(MinimumSupportedAndroidVersion));
            }

            if(MinimumSupportediOSVersion == null)
            {
                throw new ConfigurationNotFoundException(nameof(MinimumSupportediOSVersion));
            }

            if (ThrottlingEnabled == null)
            {
                throw new ConfigurationNotFoundException(nameof(ThrottlingEnabled));
            }

            if (FidoServerUrl == null)
            {
                throw new ConfigurationNotFoundException(nameof(FidoServerUrl));
            }
        }
    }
}