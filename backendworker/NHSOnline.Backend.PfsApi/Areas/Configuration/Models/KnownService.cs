using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace NHSOnline.Backend.PfsApi.Areas.Configuration.Models
{
    public abstract class KnownService
    {
        [JsonConverter(typeof(StringEnumConverter), false)]
        public JavaScriptInteractionMode JavaScriptInteractionMode { get; set; }
        
        [JsonConverter(typeof(StringEnumConverter), false)]
        public MenuTab MenuTab { get; set; }

        [JsonConverter(typeof(StringEnumConverter), false)]
        public ViewMode ViewMode { get; set; }

        public bool ShowSpinner { get; set; }

        public bool ShowThirdPartyWarning{ get; set; }
        
        public bool RequiresAssertedLoginIdentity { get; set; }
        
        public bool ValidateSession { get; set; }
    }
}