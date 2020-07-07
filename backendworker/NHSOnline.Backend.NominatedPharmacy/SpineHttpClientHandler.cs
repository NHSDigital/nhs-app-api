using System.Net.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Certificate;
using NHSOnline.Backend.Support.Settings;

namespace NHSOnline.Backend.NominatedPharmacy
{
    public class SpineHttpClientHandler : HttpClientHandler
    {
        public SpineHttpClientHandler(
            SpineLdapConfigurationSettings spineLdapConfigurationSettings,
            ILogger<SpineHttpClientHandler> logger,
            ICertificateService certificateService)
        {
            ServerCertificateCustomValidationCallback =
                    certificateService.ServerCertificateValidationHandler;

            var path = spineLdapConfigurationSettings.CertPath;
            var password = spineLdapConfigurationSettings.CertPassword;
            logger.LogInformation($"SPINE_CERT_PATH: {path}");
            var certificate = certificateService.GetCertificate(path, password);

            if (certificate != null)
            {
                ClientCertificates.Add(certificate);
            }
        }
    }
}