using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSearch.Models
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