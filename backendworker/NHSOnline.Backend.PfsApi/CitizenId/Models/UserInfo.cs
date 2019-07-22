using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;

namespace NHSOnline.Backend.PfsApi.CitizenId.Models
{
    [Serializable]
    public class UserInfo
    {
        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        [JsonProperty("gp_integration_credentials")]
        public GpIntegrationCredentials GpIntegrationCredentials { get; set; }

        [JsonProperty("nhs_number")]
        public string NhsNumber { get; set; }

        [JsonProperty("im1_token")]
        public string Im1ConnectionToken { get; set; }
        
        [JsonProperty("sub")]
        public string Subject { get; set; }
        
        [JsonProperty("birthdate")]
        public string Birthdate { get; set; }
    }
}