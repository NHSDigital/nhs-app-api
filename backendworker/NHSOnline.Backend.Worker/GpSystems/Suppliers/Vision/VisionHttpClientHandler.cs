using System;
using System.IO;
using System.Net.Http;
using System.Security.Authentication;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision
{
    public interface IVisionHttpClientHandler
    {
    }

    public class VisionHttpClientHandler : HttpClientHandler, IVisionHttpClientHandler
    {
        public VisionHttpClientHandler(IConfiguration configuration,
            ILogger<VisionHttpClientHandler>
                logger, ICertificateService certificateService)
        {
            if (configuration["ASPNETCORE_ENVIRONMENT"] != "Production")
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }
            
            var certificatePath = configuration.GetOrWarn("VISION_CERT_PATH", logger);
            var certifcatePassphrase = configuration.GetOrWarn("VISION_CERT_PASSPHRASE", logger);
            logger.LogInformation("VISION_CERT_PATH: {path}", certificatePath);

            // Get the certificate from a file
            var  certificate = certificateService.GetCertificate(certificatePath, certifcatePassphrase);

            // Set up a http handler to deal with the client certificate.
            ClientCertificateOptions = ClientCertificateOption.Manual;
            SslProtocols = SslProtocols.Tls;

            ClientCertificates.Add(certificate);
        }
    }
}
