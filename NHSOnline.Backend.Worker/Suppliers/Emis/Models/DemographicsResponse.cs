using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Suppliers.Emis.Models
{
    public class DemographicsResponse
    {
        public IEnumerable<PatientIdentifier> PatientIdentifiers { get; set; }
    }
}