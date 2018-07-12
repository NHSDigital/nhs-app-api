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
        
        public Date Date { get; set; }
        public string Description { get; set; }
        public List<string> TestResultLineItems { get; set; }
    } 
}