using System.Collections.Generic;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class OdsCodesResponse
    {
        public IEnumerable<string> OdsCodes { get; set; } = new List<string>();
    }
}
