using Newtonsoft.Json;

namespace Nhs.App.Api.Integration.Tests.Services.AccessTokenService
{
    public class AccessTokenDto
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; } = string.Empty;

        [JsonProperty("expires_in")]
        public long ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; } = string.Empty;
    }
}
