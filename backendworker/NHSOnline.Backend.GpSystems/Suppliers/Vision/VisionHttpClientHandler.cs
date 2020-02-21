using System.Net.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionHttpClientHandler : HttpClientHandler
    {

        public VisionHttpClientHandler(
            VisionConfigurationSettings visionConfigurationSettings,
            ILogger<VisionHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            ServerCertificateCustomValidationCallback =
                certificateService.ServerCertificateValidationHandler;

            var path = visionConfigurationSettings.CertificatePath;
            var password = visionConfigurationSettings.CertificatePassphrase;
            logger.LogInformation($"VISION_CERT_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
                certificateService.LogCertInfo("Vision cert info:", certificate);
            }
        }
    }
}
