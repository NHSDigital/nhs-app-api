using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Auth.UnitTests.CitizenId
{
    internal static class JwtToken
    {
        public static string Generate(IEnumerable<Claim> claims = null)  
        {  
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("This is my private key"));  
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha512);  
  
            var token = new JwtSecurityToken("https://auth.ext.signin.nhs.uk",  
                "nhs-online",  
                claims,  
                expires: DateTime.Now.AddMinutes(60),  
                signingCredentials: credentials);  
  
            return new JwtSecurityTokenHandler().WriteToken(token);  
        } 
    }
}