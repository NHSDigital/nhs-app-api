using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Worker.Support.Certificate;

namespace NHSOnline.Backend.Worker.Ndop
{
    public class NdopSigning: INdopSigning
    {
        private readonly IConfiguration _configuration;
        private readonly ICertificateService _certificateService;
        private readonly ILogger<NdopService> _logger;
        
        public NdopSigning(IConfiguration configuration,
            ILoggerFactory loggerFactory,
            ICertificateService certificateService)
        {
            _configuration = configuration;
            _certificateService = certificateService;
            _logger = loggerFactory.CreateLogger<NdopService>();
        }
        
        public SigningCredentials GetSigningCredentials()
        {
            try
            {
                var certPath = _configuration.GetOrWarn("NDOP_CERTIFICATE_PATH", _logger);
                var password = _configuration.GetOrWarn("NDOP_CERTIFICATE_PASSWORD", _logger);
                _logger.LogInformation("NDOP_CERTIFICATE_PATH: {path}", certPath);

                var certificate = _certificateService.GetCertificate(certPath, password);

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