using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class IdTokenService : IJwtTokenService<IdToken>
    {
        private readonly ITokenValidationParameterBuilder _parameterBuilder;
        private readonly ILogger<IdTokenService> _logger;
        private readonly IJwtTokenValidator _jwtTokenHandler;
        private readonly ICitizenIdSigningKeysProvider _citizenIdKeysProvider;

        public IdTokenService(ITokenValidationParameterBuilder parameterBuilder,
            IJwtTokenValidator tokenValidator,
            ICitizenIdSigningKeysProvider citizenIdKeysProvider,
            ILogger<IdTokenService> logger)
        {
            _logger = logger;
            _jwtTokenHandler = tokenValidator;
            _citizenIdKeysProvider = citizenIdKeysProvider;
            _parameterBuilder = parameterBuilder;
        }

        public async Task<Option<IdToken>> ReadToken(string token)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError("Invalid ID Token, empty token");
                return Option.None<IdToken>();
            }

            if (!_jwtTokenHandler.CanReadToken(token))
            {
                _logger.LogError("Invalid ID Token structure");
                return Option.None<IdToken>();
            }

            if (!_jwtTokenHandler.CanValidateToken)
            {
                _logger.LogError("ID Token cannot be validated");
                return Option.None<IdToken>();
            }

            try
            {
                var tokenKeyId = _jwtTokenHandler.ReadToken(token).Header.Kid;
                var signingKeysResponse = await _citizenIdKeysProvider.GetSigningKeys(tokenKeyId);

                if (!signingKeysResponse.HasValue)
                {
                    _logger.LogError("Failed to get signing keys");
                    return Option.None<IdToken>();
                }

                var signingKeys = signingKeysResponse.ValueOrFailure();
                var validationParameters = _parameterBuilder.Build(signingKeys);

                var principal = _jwtTokenHandler.ValidateToken(token, validationParameters, out _);

                var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                if (subject == null)
                {
                    _logger.LogError("Invalid ID Token; does not contain a sub");
                    return Option.None<IdToken>();
                }

                var jti = principal.FindFirstValue(JwtRegisteredClaimNames.Jti);
                if (jti == null)
                {
                    _logger.LogError("Invalid ID Token; does not contain a jti");
                    return Option.None<IdToken>();
                }
                if (string.IsNullOrWhiteSpace(jti))
                {
                    _logger.LogInformation("ID Token has empty or whitespace jti");
                }
                else
                {
                    _logger.LogInformation($"ID Token read with jti ending: {jti.Substring(jti.Length - 5)}");
                }

                var exp = principal.FindFirstValue(JwtRegisteredClaimNames.Exp);
                if (exp == null)
                {
                    _logger.LogInformation("ID Token does not contain a exp");
                }
                else
                {
                    _logger.LogInformation($"ID Token read with expiry: {exp}");
                }

                return Option.Some(new IdToken { Subject = subject, Jti = jti });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ID Token Validation Failed");
                return Option.None<IdToken>();
            }
        }
    }
}