using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class Referrals : IPatientDataModel
    { 
        public Referrals()
        { 
            Data = new List<ReferralItem>(); 
            HasAccess = true; 
            HasErrored = false; 
            HasUndeterminedAccess = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public int RecordCount => Data?.Count() ?? 0;
        public IEnumerable<ReferralItem> Data { get; set; }
        public bool HasUndeterminedAccess { get; set; }
    }
}