using System.Collections.Generic;
using NHSOnline.Backend.Worker.GpSearch.Models;

namespace NHSOnline.Backend.Worker.Areas.GpSearch.Models
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