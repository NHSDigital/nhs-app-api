using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class MedicationItem
    {        
        public DateTimeOffset Date { get; set; }
        public IEnumerable<MedicationLineItem> LineItems { get; set; }
    }
}