using System.Linq;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class Medications: IPatientDataModel
    {  
        public Medications()
        {
            HasAccess = true;
            HasErrored = false;
            Data = new MedicationsData();
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }

        public int RecordCount => (Data?.AcuteMedications.Count() ?? 0) +
                                  (Data?.CurrentRepeatMedications.Count() ?? 0) +
                                  (Data?.DiscontinuedRepeatMedications.Count() ?? 0);
        public string Errors { get; set; }
        public MedicationsData Data { get; set; }
    }
}