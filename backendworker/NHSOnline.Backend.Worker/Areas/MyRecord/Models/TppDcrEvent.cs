using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class TppDcrEvent
    {
        public TppDcrEvent()
        {
            EventItems = new List<string>();
        }

        public DateTimeOffset? Date { get; set; }
        public string LocationAndDoneBy { get; set; }
        public List<string> EventItems { get; set; }
    }
}