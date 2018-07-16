using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate
{
    public class CertificateService : ICertificateService
    {
        private readonly ILogger _logger;
        private X509Certificate2 _clientCert;

        public CertificateService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CertificateService>();
        }

        public X509Certificate2 GetCertificate(string certificatePath, string certifcatePassphrase)
        {
            try
            {
                _clientCert = new X509Certificate2(certificatePath, certifcatePassphrase);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical Error building Certificate: {certificatePath}");
                _logger.LogCritical(ex.ToString());
                throw;
            }
            return _clientCert;
        }
    }
}
