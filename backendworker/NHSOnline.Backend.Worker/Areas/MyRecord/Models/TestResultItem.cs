using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class TestResultItem
    {
        public string Id { get; set; }
        public MyRecordDate Date { get; set; }
        public string Description { get; set; }
        public List<string> AssociatedTexts { get; set; }
        public List<TestResultChildLineItem> TestResultChildLineItems { get; set; }
        
        public TestResultItem()
        {
            TestResultChildLineItems = new List<TestResultChildLineItem>();
            AssociatedTexts = new List<string>();
        }
    } 
}