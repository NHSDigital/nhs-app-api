using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Bridges.Emis.Models.PatientRecord
{
    public class AllergyMedicalRecord
    {
        public IEnumerable<AllergyResponse> Allergies { get; set; }          
    }
}