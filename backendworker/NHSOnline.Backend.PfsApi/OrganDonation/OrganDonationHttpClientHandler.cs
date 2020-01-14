using System;
using System.Net.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.PfsApi.OrganDonation
{
    public class OrganDonationHttpClientHandler : HttpClientHandler
    {
        public OrganDonationHttpClientHandler(
            IConfiguration configuration,
            ILogger<OrganDonationHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            ServerCertificateCustomValidationCallback =
                certificateService.ServerCertificateValidationHandler;

            var path = configuration.GetOrWarn("ORGAN_DONATION_CERT_PATH", logger);
            var password = configuration.GetOrWarn("ORGAN_DONATION_CERT_PASSPHRASE", logger);
            logger.LogInformation($"ORGAN_DONATION_CERT_PATH: {path}");

            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
            }
        }
    }
}
