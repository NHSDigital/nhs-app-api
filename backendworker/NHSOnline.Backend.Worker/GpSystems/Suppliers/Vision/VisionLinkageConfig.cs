using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionLinkageConfig : IVisionLinkageConfig
    {
        public Uri ApiUrl { get; }
        
        public VisionLinkageConfig(IConfiguration configuration, ILogger<VisionLinkageConfig> logger)
        {
            var apiBaseUriString = configuration.GetOrWarn("VISION_BASE_URI", logger);
            var visionLinkagePath = configuration.GetOrWarn("VISION_LINKAGE_PATH", logger);

            if (!string.IsNullOrEmpty(apiBaseUriString) && !string.IsNullOrEmpty(visionLinkagePath))
            {
                ApiUrl = new Uri($"{apiBaseUriString}{visionLinkagePath}", UriKind.Absolute);
            }
        }
    }
}