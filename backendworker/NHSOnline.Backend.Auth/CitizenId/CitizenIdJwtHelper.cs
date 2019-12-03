using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using Jose;
using Org.BouncyCastle.Crypto.Parameters;
using Org.BouncyCastle.OpenSsl;
using Org.BouncyCastle.Security;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface ICitizenIdJwtHelper
    {
        string CreateClientAuthJwt();
    }

    public class CitizenIdJwtHelper : ICitizenIdJwtHelper
    {
        private readonly ILogger<CitizenIdJwtHelper> _logger;
        private readonly RSAParameters _rsaParams;
        private readonly ICitizenIdConfig _config;
        private readonly string _audience;

        public CitizenIdJwtHelper(
            ILogger<CitizenIdJwtHelper> logger, 
            ICitizenIdConfig config)
        {
            if (config.AuthenticationType != CitizenIdAuthenticationType.Jwt)
            {
                return;
            }
            
            _logger = logger;
            _config = config;
            
            _rsaParams = ReadPrivateKey(config);
            _audience = BuildAudience(config);
        }

        private static RSAParameters ReadPrivateKey(ICitizenIdConfig config)
        {
            using (var reader = File.OpenText(config.NhsLoginKeyPath))
            {
                var passwordFinder = new PasswordFinder(config.NhsLoginKeyPassword);
                var rsaKey = (RsaPrivateCrtKeyParameters) new PemReader(reader, passwordFinder).ReadObject();
                var rsaParameters = DotNetUtilities.ToRSAParameters(rsaKey);

                return rsaParameters;
            }
        }

        private static string BuildAudience(ICitizenIdConfig config)
        {
            var audienceBuilder = new UriBuilder(config.Issuer) { Path = config.TokenPath };
            return audienceBuilder.Uri.ToString();
        }

        public string CreateClientAuthJwt()
        {
            try
            {
                _logger.LogEnter();
                
                var payload = new Dictionary<string, object>
                {
                    { "sub", _config.ClientId },
                    { "aud", _audience },
                    { "iss", _config.ClientId },
                    { "exp", DateTimeOffset.Now.AddMinutes(1).ToUnixTimeSeconds() },
                    { "jti", Guid.NewGuid() }
                };

                return CreateEncodedJwt(payload);
            }
            finally
            { 
                _logger.LogExit();
            }
        }

        public string CreateEncodedJwt(Dictionary<string, object> payload)
        {
            using (var rsa = new RSACryptoServiceProvider())
            {
                rsa.ImportParameters(_rsaParams);

                return JWT.Encode(payload, rsa, JwsAlgorithm.RS512);
            }
        }

        private class PasswordFinder : IPasswordFinder
        {
            private readonly string _password;

            public PasswordFinder(string password)
            {
                _password = password;
            }

            public char[] GetPassword()
            {
                return _password.ToCharArray();
            }
        }
    }
}