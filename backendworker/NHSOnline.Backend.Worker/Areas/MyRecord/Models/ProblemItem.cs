using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class ProblemItem
    {
        public MyRecordDate EffectiveDate { get; set; }     
        public List<ProblemLineItem> LineItems { get; set; }
        /*
        public string Term { get; set; }
        public string Significance { get; set; }
        public string Status { get; set; }
        public List<string> Notes { get; set; }
        public DateTimeOffset? ProblemEndDate { get; set; }
        */      
    }
}