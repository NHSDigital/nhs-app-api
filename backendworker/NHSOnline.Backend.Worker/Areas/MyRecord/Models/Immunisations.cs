using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class Immunisations
    {
        public Immunisations()
        {
            Data = new List<ImmunisationItem>();
            HasAccess = true;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public IEnumerable<ImmunisationItem> Data { get; set; }
    }
}