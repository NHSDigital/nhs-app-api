using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.UserInfo.Areas.UserResearch.Models
{
    public class UserResearchRequest
    {
        [JsonConverter(typeof(StringEnumConverter), false)]
        public UserResearchPreference Preference { get; set; }
    }
}
