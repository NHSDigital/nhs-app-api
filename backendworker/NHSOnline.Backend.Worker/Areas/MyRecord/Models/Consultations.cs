using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class Consultations
    {
        public Consultations()
        {
            Data = new List<ConsultationItem>();
            HasAccess = true;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public IEnumerable<ConsultationItem> Data { get; set; }   
    }
}