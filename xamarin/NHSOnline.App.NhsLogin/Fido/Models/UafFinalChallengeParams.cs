using Newtonsoft.Json;

namespace NHSOnline.App.NhsLogin.Fido.Models
{
    internal sealed class UafFinalChallengeParams
    {
        [JsonProperty("AppID")]
        public string AppId { get; set; } = string.Empty;

        [JsonProperty("challenge")]
        public string Challenge { get; set; } = string.Empty;

        [JsonProperty("facetID")]
        public string FacetId { get; set; } = string.Empty;
    }
}