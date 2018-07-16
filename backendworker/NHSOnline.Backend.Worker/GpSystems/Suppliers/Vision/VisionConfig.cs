using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public class VisionConfig : IVisionConfig
    {
        public string ApplicationProviderId { get;  }
        public string ApiUrl { get; }
        public string CertificatePath { get; }
        public string CertificatePassphrase { get; }
        public string RequestUsername { get; }

        public VisionConfig(IConfiguration configuration, ILogger<VisionConfig> logger)
        {
            ApplicationProviderId = configuration.GetOrWarn("VISION_APPLICATION_PROVIDER_ID", logger);
            ApiUrl = configuration.GetOrWarn("VISION_URI", logger);
            CertificatePath = configuration.GetOrWarn("VISION_CERT_PATH", logger);
            CertificatePassphrase = configuration.GetOrWarn("VISION_CERT_PASSPHRASE", logger);
            RequestUsername = configuration.GetOrWarn("VISION_USERNAME", logger);
        }
    }
}
