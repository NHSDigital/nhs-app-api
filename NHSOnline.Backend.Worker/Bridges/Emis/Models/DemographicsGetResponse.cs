using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models
{
    public class DemographicsGetResponse
    {
        public IEnumerable<PatientIdentifier> PatientIdentifiers { get; set; }
    }
}