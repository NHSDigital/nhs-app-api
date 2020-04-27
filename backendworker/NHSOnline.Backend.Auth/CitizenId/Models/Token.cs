using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Auth.CitizenId.Models
{
    [Serializable]
    public class Token
    {
        [JsonProperty("access_token")]
        public string AccessToken { get; set; }

        [JsonProperty("token_type")]
        public string TokenType { get; set; }

        [JsonProperty("expires_in")]
        public int ExpiresIn { get; set; }

        [JsonProperty("scope")]
        public string Scope { get; set; }

        [JsonProperty("id_Token")]
        public string IdToken { get; set; }
        
        [JsonProperty("nhs_number")]
        public string NhsNumber { get; set; }
        
        [JsonProperty("refresh_token")]
        public string RefreshToken { get; set; }
    }
}
