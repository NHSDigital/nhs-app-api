using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class ProblemLineItem
    {
        public string Text { get; set; }
        public List<string> LineItems { get; set; }
    }
}