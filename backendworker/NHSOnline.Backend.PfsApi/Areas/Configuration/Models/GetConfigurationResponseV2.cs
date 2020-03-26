using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public class GetConfigurationResponseV2
    {
        public List<string> NhsLoginLoggedInPaths { get; set; }

        public string MinimumSupportedAndroidVersion { get; set; }

        public string MinimumSupportediOSVersion { get; set; }

        public Uri FidoServerUrl { get; set; }

        public List<RootService> KnownServices { get; set; }
    }
}
 