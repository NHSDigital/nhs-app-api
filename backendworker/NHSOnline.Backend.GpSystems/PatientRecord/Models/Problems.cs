using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class Problems : IPatientDataModel
    {
        public Problems()
        {
            Data = new List<ProblemItem>();
            HasAccess = true;
            HasErrored = false;
            HasUndeterminedAccess = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public int RecordCount => Data?.Count() ?? 0;
        public IEnumerable<ProblemItem> Data { get; set; }
        public bool HasUndeterminedAccess { get; set; }
    }
}