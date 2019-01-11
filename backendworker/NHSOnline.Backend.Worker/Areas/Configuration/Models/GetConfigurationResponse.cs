using System;

namespace NHSOnline.Backend.Worker.Areas.Configuration.Models
{
    public class GetConfigurationResponse
    {
        public bool IsDeviceSupported { get; set; }
        public bool IsThrottlingEnabled { get; set; }
        public Uri FidoServerUrl { get; set; }
    }
}
