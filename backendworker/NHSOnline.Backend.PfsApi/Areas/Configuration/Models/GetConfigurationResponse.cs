using System;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public class GetConfigurationResponse
    {
        public bool IsDeviceSupported { get; set; }
        public bool IsThrottlingEnabled { get; set; }
        public Uri FidoServerUrl { get; set; }
    }
}
