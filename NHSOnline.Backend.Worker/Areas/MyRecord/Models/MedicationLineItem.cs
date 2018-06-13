using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class MedicationLineItem
    {
        public string Text { get; set; }
        public IEnumerable<string> LineItems { get; set; }
    }
}