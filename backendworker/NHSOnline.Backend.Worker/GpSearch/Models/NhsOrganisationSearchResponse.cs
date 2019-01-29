using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.Worker.GpSearch.Models
{
    public class NhsOrganisationSearchResponse
    {
        [JsonProperty(PropertyName = "@odata.count")]
        public int OrganisationCount { get; set; }
        
        [JsonProperty(PropertyName = "value")]
        public List<Organisation> Organisations { get; set; }
    }
}