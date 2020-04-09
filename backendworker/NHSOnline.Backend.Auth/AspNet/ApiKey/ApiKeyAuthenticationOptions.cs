using Microsoft.AspNetCore.Authentication;

namespace NHSOnline.Backend.Auth.AspNet.ApiKey
{
    public class ApiKeyAuthenticationOptions : AuthenticationSchemeOptions
    {
        public const string DefaultScheme = "API Key";
        public string Scheme => DefaultScheme;
        public string AuthenticationType => DefaultScheme;
    }
}