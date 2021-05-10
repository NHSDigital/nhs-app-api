using System.IdentityModel.Tokens.Jwt;

namespace NHSOnline.App.Api.Session
{
    public class AccessToken
    {
        private readonly string _rawAccessToken;

        public string Subject { get; }

        private AccessToken(string subject, string rawToken)
        {
            Subject = subject;
            _rawAccessToken = rawToken;
        }

        public static AccessToken Parse(string accessToken)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var token = tokenHandler.ReadJwtToken(accessToken);

            return new AccessToken(token.Subject, accessToken);
        }

        public string Raw()
        {
            return _rawAccessToken;
        }
    }
}