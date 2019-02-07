using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.Worker.GpSystems.OrganDonation.Models
{
    [JsonConverter(typeof(StringEnumConverter), false)]
    public enum ChoiceState
    {
        NotStated = 0,
        Yes,
        No
    }
}