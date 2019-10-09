using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class Encounters : IPatientDataModel
    { 
        public Encounters()
        { 
            Data = new List<EncounterItem>(); 
            HasAccess = true; 
            HasErrored = false; 
            HasUndeterminedAccess = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public int RecordCount => Data?.Count() ?? 0;
        public IEnumerable<EncounterItem> Data { get; set; }
        public bool HasUndeterminedAccess { get; set; }
    }
}