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

        public SigningCredentials GetSigningCredentials(string signingKeyPrefix)
        {
            try
            {
                var signingKey = _configuration.GetOrWarn($"{signingKeyPrefix}_SIGNING_KEY", _logger);
                var password = _configuration.GetOrWarn($"{signingKeyPrefix}_SIGNING_KEY_PASSPHRASE", _logger);
                _logger.LogInformation("{prefix}_SIGNING_KEY: {path}", signingKeyPrefix, signingKey);

                var certificate = _certificateService.GetCertificate(signingKey, password);

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
