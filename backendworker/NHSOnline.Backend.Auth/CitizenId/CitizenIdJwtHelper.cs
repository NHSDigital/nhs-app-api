using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
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
        string CreateAssertedLoginIdentityJwt(string idTokenJti);
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
                    { JwtRegisteredClaimNames.Sub, _config.ClientId },
                    { JwtRegisteredClaimNames.Aud, _audience },
                    { JwtRegisteredClaimNames.Iss, _config.ClientId },
                    { JwtRegisteredClaimNames.Exp, DateTimeOffset.Now.AddMinutes(1).ToUnixTimeSeconds() },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid() }
                };

                return CreateEncodedJwt(payload);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public string CreateAssertedLoginIdentityJwt(string idTokenJti)
        {
            try
            {
                _logger.LogEnter();

                var payload = new Dictionary<string, object>
                {
                    { "code", idTokenJti },
                    { JwtRegisteredClaimNames.Iss, _config.ClientId },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid() },
                    { JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds() },
                    { JwtRegisteredClaimNames.Exp, DateTimeOffset.Now.AddMinutes(1).ToUnixTimeSeconds() }
                };

                return CreateEncodedJwt(payload);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private string CreateEncodedJwt(Dictionary<string, object> payload)
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