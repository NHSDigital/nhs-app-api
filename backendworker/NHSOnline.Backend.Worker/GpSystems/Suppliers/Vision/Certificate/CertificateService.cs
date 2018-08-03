using System;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Certificate
{
    public class CertificateService : ICertificateService, IDisposable
    {
        private readonly ILogger _logger;
        private X509Certificate2 _clientCert;
        private bool _disposed;

        public CertificateService(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<CertificateService>();
        }

        public X509Certificate2 GetCertificate(string certificatePath, string certificatePassphrase)
        {
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
