using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest
{
    public class MicrotestHttpClientHandler : HttpClientHandler
    {
        public MicrotestHttpClientHandler(
            IConfiguration configuration,
            ILogger<MicrotestHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            if (!"Production".Equals(configuration["ASPNETCORE_ENVIRONMENT"], StringComparison.OrdinalIgnoreCase))
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            var path = configuration.GetOrWarn("MICROTEST_CERT_PATH", logger);
            var password = configuration.GetOrWarn("MICROTEST_CERT_PASSPHRASE", logger);
            logger.LogInformation($"MICROTEST_CERT_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
            }
        }
    }
}
