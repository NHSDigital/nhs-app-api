using Newtonsoft.Json;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public class OrganisationPostcodeSearchData
    {
        [JsonProperty(PropertyName = "top")]
        public int Top { get; set; }
        
        [JsonProperty(PropertyName = "select")]
        public string Select { get; set; }
        
        [JsonProperty(PropertyName = "filter")]
        public string Filter { get; set; }
        
        [JsonProperty(PropertyName = "search")]
        public string Search { get; set; }
        
        [JsonProperty(PropertyName = "count")]
        public bool Count { get; set; }
        
        [JsonProperty(PropertyName = "orderby")]
        public string OrderBy { get; set; }
        
        [JsonProperty(PropertyName = "queryType")]
        public string QueryType { get; set; }
        
        [JsonProperty(PropertyName = "searchMode")]
        public string SearchMode { get; set; }
    }
}