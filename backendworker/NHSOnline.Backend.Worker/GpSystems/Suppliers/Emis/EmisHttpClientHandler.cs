using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Support.Certificate;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis
{
    public class EmisHttpClientHandler : HttpClientHandler
    {
        public EmisHttpClientHandler(
            IConfiguration configuration,
            ILogger<EmisHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            if (!"Production".Equals(configuration["ASPNETCORE_ENVIRONMENT"], StringComparison.OrdinalIgnoreCase))
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            var path = configuration.GetOrWarn("EMIS_CERTIFICATE_PATH", logger);
            var password = configuration.GetOrWarn("EMIS_CERTIFICATE_PASSWORD", logger);
            logger.LogInformation($"EMIS_CERTIFICATE_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
            }
        }
    }
}