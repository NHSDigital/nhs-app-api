using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Auth.CitizenId.Models
{
    [Serializable]
    public class GpRegistrationDetails
    {
        [JsonProperty("gp_ods_code")]
        public string OdsCode { get; set; }
    }
}
