using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestHttpClientHandler : HttpClientHandler
    {
        public MicrotestHttpClientHandler(
            MicrotestConfigurationSettings configurationSettings,
            ILogger<MicrotestHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            ServerCertificateCustomValidationCallback =
                certificateService.ServerCertificateValidationHandler;

            var path = configurationSettings.CertificatePath;
            var password = configurationSettings.CertificatePassphrase;
            logger.LogInformation($"MICROTEST_CERT_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
                certificateService.LogCertInfo("Microtest cert info:", certificate);
            }
        }
    }
}
