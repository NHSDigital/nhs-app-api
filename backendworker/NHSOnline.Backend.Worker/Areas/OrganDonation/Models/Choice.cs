using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.Worker.Areas.OrganDonation.Models
{
    public class Choice
    {
        public string Name { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public ChoiceState Value { get; set; }
    }
}