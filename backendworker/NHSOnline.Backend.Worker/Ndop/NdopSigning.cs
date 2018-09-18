using System;
using System.IO;
using System.Security.Cryptography.X509Certificates;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Worker.Ndop
{
    public class NdopSigning: INdopSigning
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<NdopService> _logger;
        
        public NdopSigning(IConfiguration configuration, ILoggerFactory loggerFactory)
        {
            _configuration = configuration;
            _logger = loggerFactory.CreateLogger<NdopService>();
        }
        
        public SigningCredentials GetSigningCredentials()
        {
            try
            {
                var certPath = _configuration.GetOrWarn("NDOP_CERTIFICATE_PATH", _logger);
                var password = _configuration.GetOrWarn("NDOP_CERTIFICATE_PASSWORD", _logger);

                _logger.LogInformation("NDOP_CERTIFICATE_PATH: {path}", certPath);
                if (string.IsNullOrEmpty(certPath) || !File.Exists(certPath) || string.IsNullOrEmpty(password))
                {
                    _logger.LogError("Could not get Ndop certificate due to missing certificate path or password.");
                    return null;
                }
                
                var certificate = new X509Certificate2(certPath, password);

                var key = new X509SecurityKey(certificate);
                
                return new SigningCredentials(key, SecurityAlgorithms.RsaSha256);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error creating signing credentials for JWT Token");
                return null;
            }
        }
    }
}