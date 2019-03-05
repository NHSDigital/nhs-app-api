using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.PfsApi.OrganDonation.Models
{
    [JsonConverter(typeof(StringEnumConverter), false)]
    public enum ChoiceState
    {
        NotStated = 0,
        Yes,
        No
    }
}