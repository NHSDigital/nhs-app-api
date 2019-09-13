using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class MedicalHistories: IPatientDataModel
    {
        public MedicalHistories()
        {
            Data = new List<MedicalHistoryItem>();
            HasAccess = true;
            HasErrored = false;
            HasUndeterminedAccess = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public int RecordCount => Data?.Count() ?? 0;
        public IEnumerable<MedicalHistoryItem> Data { get; set; }
        public bool HasUndeterminedAccess { get; set; }
    }
}