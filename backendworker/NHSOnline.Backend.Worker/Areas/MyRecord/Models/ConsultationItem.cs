using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class ConsultationItem
    {
        public ConsultationItem()
        {
            ConsultationHeaders = new List<ConsultationHeaderItem>();
        }
        
        public MyRecordDate EffectiveDate { get; set; }
        public string ConsultantLocation { get; set; }
        public List<ConsultationHeaderItem> ConsultationHeaders { get; set; }
    }
}