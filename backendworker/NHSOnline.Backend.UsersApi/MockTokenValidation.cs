using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.UsersApi
{
    public class MockTokenValidation : ISecurityTokenValidator
    {
        private readonly JwtSecurityTokenHandler _tokenHandler;

        public bool CanValidateToken => true;
        public int MaximumTokenSizeInBytes { get; set; } = TokenValidationParameters.DefaultMaximumTokenSizeInBytes;

        public MockTokenValidation()
        {
            _tokenHandler = new JwtSecurityTokenHandler();
        }


        public bool CanReadToken(string securityToken)
        {
            return _tokenHandler.CanReadToken(securityToken);
        }

        public ClaimsPrincipal ValidateToken(string securityToken, TokenValidationParameters validationParameters,
            out SecurityToken validatedToken)
        {
            var claims = new List<Claim>();

            var userIdentity = new ClaimsIdentity(claims, "Passport");

            validatedToken = new JwtSecurityToken(securityToken);

            return new ClaimsPrincipal(userIdentity);
        }
    }
}