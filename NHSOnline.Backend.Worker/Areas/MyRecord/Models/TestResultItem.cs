using System;
using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class TestResultItem
    {
        public TestResultItem()
        {
            TestResultLineItems = new List<string>();
        }
        public Date EffectiveDate { get; set; }
        public string Term { get; set; }
        public List<string> TestResultLineItems { get; set; }
    }
}