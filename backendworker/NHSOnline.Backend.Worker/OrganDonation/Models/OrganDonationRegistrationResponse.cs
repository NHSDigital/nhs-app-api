using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.Worker.OrganDonation.Models
{
    public class OrganDonationRegistrationResponse
    {
        public string Identifier { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter), false)]
        public State State { get; set; }
    }
}