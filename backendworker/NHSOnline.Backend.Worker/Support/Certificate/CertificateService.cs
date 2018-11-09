using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.Certificate
{
    public class CertificateService : ICertificateService
    {
        private readonly ILogger _logger;

        public CertificateService(ILogger<CertificateService> logger)
        {
            _logger = logger;
        }

        public X509Certificate2 GetCertificate(string certificatePath, string certificatePassphrase)
        {
            if (!CheckValid(certificatePath, certificatePassphrase))
            {
                return null;
            }

            X509Certificate2 _clientCert;
            try
            {
                _clientCert = new X509Certificate2(certificatePath, certificatePassphrase);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical Error building Certificate: {certificatePath}");
                _logger.LogCritical(ex.ToString());
                throw;
            }
            return _clientCert;
        }

        private bool CheckValid(string certificatePath, string certificatePassphrase)
        {
            var valid = true;
            if (string.IsNullOrEmpty(certificatePath))
            {
                _logger.LogError("Could not add client certificate due to missing certificate path.");
                valid= false;
            }
            if (string.IsNullOrEmpty(certificatePassphrase))
            {
                _logger.LogError("Could not add client certificate due to missing certificate passphrase.");
                valid= false;
            }
            if (!File.Exists(certificatePath))
            {
                _logger.LogError("Could not add client certificate due to file not existing in certificate path.");
                valid= false;
            }
            return valid;
        }
    }
}
