using System;
using System.Security.Claims;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public class IdTokenService : IJwtTokenService<IdToken>
    {
        private readonly ITokenValidationParameterBuilder _parameterBuilder;
        private readonly ILogger<IdTokenService> _logger;
        private readonly ISecurityTokenValidator _jwtTokenHandler;

        public IdTokenService(ITokenValidationParameterBuilder parameterBuilder, ISecurityTokenValidator tokenValidator,
            ILogger<IdTokenService> logger)
        {
            _logger = logger;
            _jwtTokenHandler = tokenValidator;
            _parameterBuilder = parameterBuilder;
        }

        public Option<IdToken> ReadToken(string token, JsonWebKeySet signingKeys)
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
                var validationParameters = _parameterBuilder.Build(signingKeys);
                var principal = _jwtTokenHandler.ValidateToken(token, validationParameters, out _);
                var subject = principal.FindFirstValue(ClaimTypes.NameIdentifier);
                
                if (subject == null)
                {
                    _logger.LogError("Invalid ID Token; does not contain a sub");
                    return Option.None<IdToken>();
                }
                
                return Option.Some(new IdToken { Subject = subject });
            }
            catch (Exception e)
            {
                _logger.LogError(e, "ID Token Validation Failed");
                return Option.None<IdToken>();
            }
        }
    }
}