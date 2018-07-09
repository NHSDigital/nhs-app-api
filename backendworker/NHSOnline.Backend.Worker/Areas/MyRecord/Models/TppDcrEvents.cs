using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class TppDcrEvents
    {
        public TppDcrEvents()
        {
            HasAccess = true;
            HasErrored = false;
            Data = new List<TppDcrEvent>();
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public string Errors { get; set; }
        public List<TppDcrEvent> Data { get; set; }
    }
}