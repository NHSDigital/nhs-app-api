using System;

namespace NHSOnline.App.Api.Client.Configuration
{
    internal class GetConfigurationResponse
    {
        public GetConfigurationResponse(
            Version minimumSupportedAndroidVersion,
            Version minimumSupportediOSVersion)
        {
            MinimumSupportedAndroidVersion = minimumSupportedAndroidVersion;
            MinimumSupportediOSVersion = minimumSupportediOSVersion;
        }

        public Version MinimumSupportedAndroidVersion { get; }

        public Version MinimumSupportediOSVersion { get; }
    }
}