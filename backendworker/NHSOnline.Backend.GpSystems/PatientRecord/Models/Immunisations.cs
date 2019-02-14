using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class Immunisations: IPatientDataModel
    {
        public Immunisations()
        {
            Data = new List<ImmunisationItem>();
            HasAccess = true;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public IEnumerable<ImmunisationItem> Data { get; set; }
    }
}