using System;
using NHSOnline.App.Api.Client.Configuration;

namespace NHSOnline.App.Api.Configuration
{
    public sealed class VersionConfiguration
    {
        private readonly GetConfigurationResponse _response;

        internal VersionConfiguration(GetConfigurationResponse response)
        {
            _response = response;
        }

        public Version MinimumSupportedAndroidVersion => _response.MinimumSupportedAndroidVersion;
        public Version MinimumSupportediOSVersion => _response.MinimumSupportediOSVersion;
    }
}