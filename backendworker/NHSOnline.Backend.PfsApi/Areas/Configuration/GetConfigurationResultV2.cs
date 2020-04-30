using System;
using System.Collections.Generic;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration
{
    public sealed class GetConfigurationResultV2
    {
        public GetConfigurationResultV2(List<string> nhsLoginLoggedInPaths,
            Uri fidoServerUrl,
            string minimumSupportedAndroidVersion,
            string minimumSupportediOSVersion,
            List<RootService> knownServices)
        {
            Response = new GetConfigurationResponseV2
            {
                NhsLoginLoggedInPaths = nhsLoginLoggedInPaths,
                FidoServerUrl = fidoServerUrl,
                MinimumSupportedAndroidVersion = minimumSupportedAndroidVersion,
                MinimumSupportediOSVersion = minimumSupportediOSVersion,
                KnownServices = knownServices,
            };
        }

        public GetConfigurationResponseV2 Response { get; }
    }
}