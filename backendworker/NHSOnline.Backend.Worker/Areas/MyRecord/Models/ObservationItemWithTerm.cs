using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class ObservationItemWithTerm
    {
        public ObservationItemWithTerm()
        {
            AssociatedTexts = new List<string>();    
        }
        
        public string Term { get; set; }
        public List<string> AssociatedTexts { get; set; }
    }
}