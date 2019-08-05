using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis
{
    public class EmisHttpClientHandler : HttpClientHandler
    {
        public EmisHttpClientHandler(
            IConfiguration configuration,
            EmisConfigurationSettings emisConfigurationSettings,
            ILogger<EmisHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            if (!"Production".Equals(configuration["ASPNETCORE_ENVIRONMENT"], StringComparison.OrdinalIgnoreCase))
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            var path = emisConfigurationSettings.CertificatePath;
            var password = emisConfigurationSettings.CertificatePassphrase;
            logger.LogInformation($"EMIS_CERTIFICATE_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
            }
        }
    }
}