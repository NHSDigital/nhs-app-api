using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.Certificate
{
    public class CertificateService : ICertificateService, IDisposable
    {
        private readonly ILogger _logger;
        private X509Certificate2 _clientCert;
        private bool _disposed;

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

            try
            {
                if (_disposed)
                {
                    throw new ObjectDisposedException(GetType().FullName);
                }
                
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
        
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        ~CertificateService()
        {
            Dispose(false);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (_disposed)
            {
                return;
            }

            if (disposing)
            {
                _clientCert.Dispose();
            }

            _disposed = true;
        }
    }
}
