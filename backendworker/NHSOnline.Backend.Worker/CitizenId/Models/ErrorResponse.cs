using System;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.CitizenId.Models
{
    [Serializable]
    public class ErrorResponse
    {
        public string Error { get; set; }

        [JsonProperty("error_description")]
        public string ErrorDescription { get; set; }
    }
}
