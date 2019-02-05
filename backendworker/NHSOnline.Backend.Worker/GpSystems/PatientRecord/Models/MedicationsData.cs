using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord.Models
{
    public class MedicationsData
    {
        public MedicationsData()
        {
            AcuteMedications = new List<MedicationItem>();
            CurrentRepeatMedications = new List<MedicationItem>();
            DiscontinuedRepeatMedications = new List<MedicationItem>();
        }
        
        public IEnumerable<MedicationItem> AcuteMedications { get; set; }
        public IEnumerable<MedicationItem> CurrentRepeatMedications { get; set; }
        public IEnumerable<MedicationItem> DiscontinuedRepeatMedications { get; set; }
    }
}