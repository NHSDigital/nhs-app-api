using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class PatientDocuments : IPatientDataModel
    {
        public PatientDocuments()
        {
            Data = new List<DocumentItem>();
            HasAccess = true;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public int RecordCount => Data?.Count() ?? 0;
        public IEnumerable<DocumentItem> Data { get; set; }
    }
}