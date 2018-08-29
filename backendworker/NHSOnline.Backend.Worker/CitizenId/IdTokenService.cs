using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json;
using NHSOnline.Backend.Worker.CitizenId.Models;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public class IdTokenService : IJwtTokenService<UserProfile>
    {
        private readonly ITokenValidationParameterBuilder _parameterBuilder;
        private readonly ILogger<IdTokenService> _logger;
        private readonly ISecurityTokenValidator _jwtTokenHandler;

        public IdTokenService(ITokenValidationParameterBuilder parameterBuilder,ISecurityTokenValidator tokenValidator, ILogger<IdTokenService> logger)
        {
            _logger = logger;
            _jwtTokenHandler = tokenValidator;
            _parameterBuilder = parameterBuilder;
        }

        public Option<UserProfile> ReadToken(string token, JsonWebKeySet signingKeys)
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                _logger.LogError("Invalid IdToken, empty token");
                return Option.None<UserProfile>();
            }
            
            if (!_jwtTokenHandler.CanReadToken(token))
            {
                _logger.LogError("Invalid IdToken structure");
                return Option.None<UserProfile>();
            }
            
            if (!_jwtTokenHandler.CanValidateToken)
            {
                _logger.LogError("Id Token cannot be validated");
                return Option.None<UserProfile>();
            }

            try
            {
                var validationParameters = _parameterBuilder.Build(signingKeys);
                
                SecurityToken secKey;
                var principal = _jwtTokenHandler.ValidateToken(token, validationParameters, out secKey);

                var im1Token = principal.FindFirstValue(Constants.CitizenIdClaimTypes.Im1ConnectionTokenClaim);
                var odsCode = principal.FindFirstValue(Constants.CitizenIdClaimTypes.OdscodeClaim);
                var dateOfBirth = principal.FindFirstValue(ClaimTypes.DateOfBirth);
                var nhsNumber = principal.FindFirstValue(Constants.CitizenIdClaimTypes.NhsNumber);
            
                if (im1Token == null)
                {
                    _logger.LogError("Invalid IdToken, does not contain an im1_token");
                    return Option.None<UserProfile>();
                }

                if (odsCode == null)
                {
                    _logger.LogError("Invalid IdToken, does not contain an ods_code");
                    return Option.None<UserProfile>();
                }

                return Option.Some(new UserProfile()
                {
                    Im1ConnectionToken = im1Token,
                    OdsCode = odsCode,
                    DateOfBirth = dateOfBirth,
                    NhsNumber = nhsNumber,
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e,"IdToken Validation Failed");
                return Option.None<UserProfile>();
            }
        }
    }
}