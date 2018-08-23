using System;
using System.Collections.Generic;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Worker.CitizenId
{
    public interface ITokenValidationParameterBuilder
    {
        TokenValidationParameters Build(JsonWebKeySet keys);
    }

    public class TokenValidationParameterBuilder : ITokenValidationParameterBuilder
    {
        private readonly string _issuer;
        private readonly string _audience;
    
        public TokenValidationParameterBuilder(ICitizenIdConfig config)
        {
            _issuer = config.Issuer;
            _audience = config.ClientId;
        }

        public TokenValidationParameters Build(JsonWebKeySet keys)
        {
            return new TokenValidationParameters
            {
                IssuerSigningKeys = keys.GetSigningKeys(),
                ValidAudience = _audience,
                ValidateAudience = true,
                ValidateIssuer = true,
                ValidIssuer = _issuer
            };
        }
    }
}