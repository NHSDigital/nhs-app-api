using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionDirectServicesConfig : IVisionDirectServicesConfig
    {
        public Uri ApiUrl { get; }

        public VisionDirectServicesConfig(IConfiguration configuration, ILogger<VisionDirectServicesConfig> logger)
        {
            var apiBaseUriString = configuration.GetOrWarn("VISION_BASE_URL", logger);

            if (!string.IsNullOrEmpty(apiBaseUriString))
            {
                ApiUrl = new Uri(apiBaseUriString, UriKind.Absolute);
            }
        }
    }
}
