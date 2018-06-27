using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.CitizenId.Models
{
    [Serializable]
    public class UserInfo
    {
        public string Name { get; set; }

        [JsonProperty("given_name")]
        public string GivenName { get; set; }

        [JsonProperty("family_name")]
        public string FamilyName { get; set; }

        public string Email { get; set; }

        // TODO - NHSO-454 - awaiting confirmation of field name from Citizen ID.
        [JsonProperty("im1_connection_token")]
        public string Im1ConnectionToken { get; set; }

        // TODO - NHSO-454 - awaiting confirmation of field name from Citizen ID.
        [JsonProperty("ods_code")]
        public string OdsCode { get; set; }
    }
}
