using Newtonsoft.Json;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public class PostcodeSearchData
    {
        [JsonProperty(PropertyName = "top")]
        public int Top { get; set; }
        
        [JsonProperty(PropertyName = "search")]
        public string Search { get; set; }
        
        [JsonProperty(PropertyName = "count")]
        public bool Count { get; set; }
        
        [JsonProperty(PropertyName = "filter")]
        public string Filter { get; set; }
    }
}