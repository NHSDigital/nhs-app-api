using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class MedicationLineItem
    {
        public MedicationLineItem()
        {
            LineItems = new List<string>();
        }
        public string Text { get; set; }
        public List<string> LineItems { get; set; }
    }
}