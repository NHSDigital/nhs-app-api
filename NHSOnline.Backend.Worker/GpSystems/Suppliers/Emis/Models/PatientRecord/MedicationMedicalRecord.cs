using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord
{
    public class MedicationMedicalRecord
    {      
        public IEnumerable<MedicationResponse> Medication { get; set; }  
    }
}