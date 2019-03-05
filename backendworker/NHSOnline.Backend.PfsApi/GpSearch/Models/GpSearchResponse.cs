using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.GpSearch.Models
{
    public class GpSearchResponse
    {
        public GpSearchResponse()
        {
            Organisations = new List<Organisation>();
        }
        
        public List<Organisation> Organisations { get; set; }
        public int OrganisationQueryCount { get; set; }
    }
}