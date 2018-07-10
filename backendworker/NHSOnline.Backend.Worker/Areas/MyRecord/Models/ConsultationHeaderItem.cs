using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class ConsultationHeaderItem
    {
        public ConsultationHeaderItem()
        {
            Observations = new List<ObservationItem>();
        }
        
        public string Header { get; set; }
        public List<ObservationItem> Observations { get; set; }
    }
}