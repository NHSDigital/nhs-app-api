using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Support.Certificate
{
    public class CertificateService : ICertificateService
    {
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public CertificateService(ILogger<CertificateService> logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
        }

        public X509Certificate2 GetCertificate(string certificatePath, string certificatePassphrase)
        {
            if (!CheckValid(certificatePath, certificatePassphrase))
            {
                return null;
            }

            try
            {
                return new X509Certificate2(certificatePath, certificatePassphrase);
            }
            catch (Exception ex)
            {
                _logger.LogCritical($"Critical Error building Certificate: {certificatePath}");
                _logger.LogCritical(ex.ToString());
                throw;
            }
        }
        
        public bool ServerCertificateValidationHandler(object sender, X509Certificate certificate,
            X509Chain chain, System.Net.Security.SslPolicyErrors sslPolicyErrors)
        {
            {
                // Overriding the default server certificate validation from:
                //   https://github.com/dsbenghe/Novell.Directory.Ldap.NETStandard/blob/master/src/Novell.Directory.Ldap.NETStandard/Connection.cs#L475
                // The code is the same but we have added logs to help get to the bottom of certificate problems, if we encounter them.

                var success = false;

                _logger.LogInformation($"ServerCertificateValidationHandler - SslPolicyErrors: {sslPolicyErrors}");

                if (sslPolicyErrors != System.Net.Security.SslPolicyErrors.None)
                {
                    var inProduction = "Production".Equals(_configuration["ASPNETCORE_ENVIRONMENT"],
                        StringComparison.OrdinalIgnoreCase);
                    _logger.LogInformation($"Running in Production Mode: {inProduction}");
                    
                    if (!inProduction && sslPolicyErrors == System.Net.Security.SslPolicyErrors.RemoteCertificateNameMismatch)
                    {
                        _logger.LogError($"SSL policy errors={sslPolicyErrors.ToString()}, ignoring");
                        success = true;
                    }
                    else
                    {
                        foreach (var item in chain.ChainStatus)
                        {
                            _logger.LogError($"Certificate validation was not successful. " +
                                             $"SSL policy errors={sslPolicyErrors.ToString()}, Status={item.Status}, StatusInformation={item.StatusInformation}");
                        }
                    }
                }
                else
                {
                    success = true;
                }

                return success;
            }
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
