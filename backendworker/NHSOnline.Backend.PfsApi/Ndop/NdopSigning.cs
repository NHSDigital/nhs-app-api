using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;

namespace NHSOnline.Backend.PfsApi.Ndop
{
    public class NdopSigning: INdopSigning
    {
        private readonly IConfiguration _configuration;
        private readonly ICertificateService _certificateService;
        private readonly ILogger<NdopSigning> _logger;
        
        public NdopSigning(IConfiguration configuration,
            ILogger<NdopSigning> logger,
            ICertificateService certificateService)
        {
            _configuration = configuration;
            _certificateService = certificateService;
            _logger = logger;
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