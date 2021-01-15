using System;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace NHSOnline.Backend.Support.Certificate
{
    public class Signing : ISigning
    {
        private readonly IConfiguration _configuration;
        private readonly ICertificateService _certificateService;
        private readonly ILogger<Signing> _logger;

        public Signing(IConfiguration configuration,
            ILogger<Signing> logger,
            ICertificateService certificateService)
        {
            _configuration = configuration;
            _certificateService = certificateService;
            _logger = logger;
        }

        public SigningCredentials GetSigningCredentials(string certPrefix)
        {
            try
            {
                var certPath = _configuration.GetOrWarn($"{certPrefix}_CERT_PATH", _logger);
                var password = _configuration.GetOrWarn($"{certPrefix}_CERT_PASSPHRASE", _logger);
                _logger.LogInformation("{prefix}_CERT_PATH: {path}", certPrefix, certPath);

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

        public RSAParameters GetRsaParameters(IKeyConfig keyConfig)
        {
            using var reader = File.OpenText(keyConfig.KeyPath);

            var passwordFinder = new PasswordFinder(keyConfig.Password);
            var rsaKey = (RsaPrivateCrtKeyParameters) new PemReader(reader, passwordFinder).ReadObject();
            var rsaParameters = DotNetUtilities.ToRSAParameters(rsaKey);

            return rsaParameters;
        }
    }
}