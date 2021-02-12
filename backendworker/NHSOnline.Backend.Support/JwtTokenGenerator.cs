using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Jose;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Support
{
    public class JwtTokenGenerator : IJwtTokenGenerator
    {
        public string GenerateJwtSecurityToken(
            SigningCredentials signingCredentials,
            DateTime expires,
            string claimAudience = null,
            string claimIssuer = null,
            IEnumerable<Claim> claims = null)
        {
            var token = new JwtSecurityToken(
                audience: claimAudience,
                issuer: claimIssuer,
                claims: claims,
                expires: expires,
                signingCredentials: signingCredentials);

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        public string GenerateJwtSecurityToken(RSAParameters parameters, Dictionary<string, object> payload)
        {
            using var rsa = new RSACryptoServiceProvider();

            rsa.ImportParameters(parameters);

            return JWT.Encode(payload, rsa, JwsAlgorithm.RS512);
        }

        public string DecodeJwtSecurityToken(RSAParameters parameters, string cookie)
        {
            using var rsa = new RSACryptoServiceProvider();

            rsa.ImportParameters(parameters);

            return JWT.Decode(cookie, rsa, JwsAlgorithm.RS512);
        }
    }
}
