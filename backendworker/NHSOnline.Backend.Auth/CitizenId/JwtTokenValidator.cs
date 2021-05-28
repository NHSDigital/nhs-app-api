using System.IdentityModel.Tokens.Jwt;
using Microsoft.IdentityModel.Tokens;

namespace NHSOnline.Backend.Auth.CitizenId
{
    public interface IJwtTokenValidator : ISecurityTokenValidator
    {
        public JwtSecurityToken ReadToken(string rawToken);
    }

    public class JwtTokenValidator : JwtSecurityTokenHandler, IJwtTokenValidator
    {
        public new JwtSecurityToken ReadToken(string rawToken) => base.ReadToken(rawToken) as JwtSecurityToken;
    }
}
