using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models
{
    public class Allergies: IPatientDataModel
    {
        public Allergies()
        {
            Data = new List<AllergyItem>();
            HasAccess = true;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public IEnumerable<AllergyItem> Data { get; set; }
    }
}