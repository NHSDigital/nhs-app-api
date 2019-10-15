using System;
using System.Net.Http;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class SpineHttpClientHandler : HttpClientHandler
    {
        private readonly ILogger<SpineHttpClientHandler> _logger;
        public SpineHttpClientHandler(
            SpineLdapConfigurationSettings spineLdapConfigurationSettings,
            ILogger<SpineHttpClientHandler> logger,
            ICertificateService certificateService)
        {

            _logger = logger;
            ServerCertificateCustomValidationCallback =
                    certificateService.ServerCertificateValidationHandler;
            
            var path = spineLdapConfigurationSettings.CertPath;
            var password = spineLdapConfigurationSettings.CertPassword;
            logger.LogInformation($"SPINE_CERTIFICATE_PATH: {path}");
            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                logger.LogInformation("about to add Spine cert.");
                ClientCertificates.Add(certificate);
                logger.LogInformation("Spine cert added successfully");
            }

        }
    }
}