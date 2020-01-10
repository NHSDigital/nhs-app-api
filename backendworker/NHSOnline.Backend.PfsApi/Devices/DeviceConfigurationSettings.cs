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
        
        public Uri WebAppBaseUrl { get; set; }

        public DeviceConfigurationSettings() {}
        
        public DeviceConfigurationSettings(string minimumSupportedAndroidVersion,
            string minimumSupportediOSVersion, Uri fidoServerUrl, string throttlingEnabled, Uri webAppBaseUrl)
        {
            MinimumSupportedAndroidVersion = minimumSupportedAndroidVersion;
            MinimumSupportediOSVersion  = minimumSupportediOSVersion;
            FidoServerUrl = fidoServerUrl;
            ThrottlingEnabled = throttlingEnabled;
            WebAppBaseUrl = webAppBaseUrl;
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
            
            if (WebAppBaseUrl == null)
            {
                throw new ConfigurationNotFoundException(nameof(WebAppBaseUrl));
            }
        }
    }
}