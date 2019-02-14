using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Vision
{
    public class VisionHttpClientHandler : HttpClientHandler
    {

        public VisionHttpClientHandler(
            IConfiguration configuration,
            ILogger<VisionHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            if (!"Production".Equals(configuration["ASPNETCORE_ENVIRONMENT"], StringComparison.OrdinalIgnoreCase))
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            var path = configuration.GetOrWarn("VISION_CERT_PATH", logger);
            var password = configuration.GetOrWarn("VISION_CERT_PASSPHRASE", logger);
            logger.LogInformation($"VISION_CERT_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
            }
        }
    }
}
