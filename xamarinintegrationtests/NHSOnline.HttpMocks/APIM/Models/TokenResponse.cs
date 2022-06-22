using Newtonsoft.Json;

namespace NHSOnline.HttpMocks.APIM.Models
{
    public class TokenResponse
    {
        [JsonProperty("access_token")]
        public string? AccessToken { get; set; }

        [JsonProperty("expires_in")]
        public string? ExpiresIn { get; set; }

        [JsonProperty("token_type")]
        public string? TokenType { get; set; }

        [JsonProperty("issued_token_type")]
        public string? IssuedTokenType { get; set; }
    }
}