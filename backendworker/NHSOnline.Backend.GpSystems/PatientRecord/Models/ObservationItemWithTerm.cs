using System.Collections.Generic;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class ObservationItemWithTerm
    {
        public ObservationItemWithTerm()
        {
            AssociatedTexts = new List<string>();    
        }
        
        public string Term { get; set; }
        public List<string> AssociatedTexts { get; set; }
        public string EventGuid { get; set; }
        public long? CodeId { get; set; }
        public string ObservationType { get; set; }
    }
}