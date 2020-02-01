using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp.Client
{
    public class TppHttpClientHandler : HttpClientHandler
    {
        public TppHttpClientHandler(
            TppConfigurationSettings configurationSettings,
            ILogger<TppHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            ServerCertificateCustomValidationCallback =
                certificateService.ServerCertificateValidationHandler;

            var path = configurationSettings.CertificatePath;
            var password = configurationSettings.CertificatePassphrase;
            logger.LogInformation($"TPP_CERTIFICATE_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
                certificateService.LogCertInfo("Tpp cert info:", certificate);
            }
        }
    }
}
