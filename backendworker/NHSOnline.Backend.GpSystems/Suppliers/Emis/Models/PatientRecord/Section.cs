using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class Section
    {
        public string Header { get; set; }
        public string Code { get; set; }
        public int? PageNumber { get; set; }
        public List<Observation> Observations { get; set; }
    }
}