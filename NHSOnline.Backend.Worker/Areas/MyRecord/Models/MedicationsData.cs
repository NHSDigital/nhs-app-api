using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class MedicationsData
    {     
        public IEnumerable<MedicationItem> AcuteMedications { get; set; }
        public IEnumerable<MedicationItem> CurrentRepeatMedications { get; set; }
        public IEnumerable<MedicationItem> DiscontinuedRepeatMedications { get; set; }
    }
}