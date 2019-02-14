using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.GpSystems.Suppliers.Tpp
{
    public class TppHttpClientHandler : HttpClientHandler
    {
        public TppHttpClientHandler(
            IConfiguration configuration,
            ILogger<TppHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            if (!"Production".Equals(configuration["ASPNETCORE_ENVIRONMENT"], StringComparison.OrdinalIgnoreCase))
            {
                ServerCertificateCustomValidationCallback = (message, cert, chain, errors) => true;
            }

            var path = configuration.GetOrWarn("TPP_CERTIFICATE_PATH", logger);
            var password = configuration.GetOrWarn("TPP_CERTIFICATE_PASSWORD", logger);
            logger.LogInformation($"TPP_CERTIFICATE_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
            }
        }
    }
}
