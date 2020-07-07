using System.Net.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisHttpClientHandler : HttpClientHandler
    {
        public EmisHttpClientHandler(
            EmisConfigurationSettings emisConfigurationSettings,
            ILogger<EmisHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            ServerCertificateCustomValidationCallback = certificateService.ServerCertificateValidationHandler;

            var path = emisConfigurationSettings.CertificatePath;
            var password = emisConfigurationSettings.CertificatePassphrase;
            logger.LogInformation($"EMIS_CERT_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
                certificateService.LogCertInfo("Emis cert info:", certificate);
            }
        }
    }
}