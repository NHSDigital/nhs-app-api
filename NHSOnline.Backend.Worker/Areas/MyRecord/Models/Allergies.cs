using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class Allergies
    {
        public Allergies()
        {
            Data = new List<AllergyItem>();
            HasAccess = false;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public IEnumerable<AllergyItem> Data { get; set; }
    }
}