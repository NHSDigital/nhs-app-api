using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class Problems
    {
        public Problems()
        {
            Data = new List<ProblemItem>();
            HasAccess = true;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public IEnumerable<ProblemItem> Data { get; set; }       
    }
}