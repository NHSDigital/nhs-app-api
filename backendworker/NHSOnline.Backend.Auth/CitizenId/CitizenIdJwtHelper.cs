using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Cryptography;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Certificate;

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
        private readonly ICitizenIdConfig _citizenIdConfig;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly string _audience;

        public CitizenIdJwtHelper(
            ILogger<CitizenIdJwtHelper> logger,
            ICitizenIdConfig citizenIdConfig,
            AuthSigningConfig authSigningConfig,
            IJwtTokenGenerator jwtTokenGenerator,
            ISigning signing)
        {
            _logger = logger;
            _citizenIdConfig = citizenIdConfig;
            _jwtTokenGenerator = jwtTokenGenerator;

            _rsaParams = signing.GetRsaParameters(authSigningConfig);
            _audience = BuildAudience(citizenIdConfig);
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
                    { JwtRegisteredClaimNames.Sub, _citizenIdConfig.ClientId },
                    { JwtRegisteredClaimNames.Aud, _audience },
                    { JwtRegisteredClaimNames.Iss, _citizenIdConfig.ClientId },
                    { JwtRegisteredClaimNames.Exp, DateTimeOffset.Now.AddMinutes(1).ToUnixTimeSeconds() },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid() }
                };

                return _jwtTokenGenerator.GenerateJwtSecurityToken(_rsaParams, payload);
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
                    { JwtRegisteredClaimNames.Iss, _citizenIdConfig.ClientId },
                    { JwtRegisteredClaimNames.Jti, Guid.NewGuid() },
                    { JwtRegisteredClaimNames.Iat, DateTimeOffset.Now.ToUnixTimeSeconds() },
                    { JwtRegisteredClaimNames.Exp, DateTimeOffset.Now.AddMinutes(1).ToUnixTimeSeconds() }
                };

                return _jwtTokenGenerator.GenerateJwtSecurityToken(_rsaParams, payload);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}