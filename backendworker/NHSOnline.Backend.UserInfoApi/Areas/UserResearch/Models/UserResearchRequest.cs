using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.UserInfoApi.Areas.UserResearch.Models
{
    public class UserResearchRequest
    {
        [JsonConverter(typeof(StringEnumConverter), false)]
        public UserResearchPreference Preference { get; set; }
    }
}
