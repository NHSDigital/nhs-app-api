using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Support
{
    public interface IJwtTokenGenerator
    {
        string GenerateJwtSecurityToken(SigningCredentials signingCredentials, DateTime expires,
            string claimAudience = null, string claimIssuer = null, IEnumerable<Claim> claims = null);

        string GenerateJwtSecurityToken(RSAParameters parameters, Dictionary<string, object> payload);
    }
}