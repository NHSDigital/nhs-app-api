using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class TestResultChildLineItem
    {
        public TestResultChildLineItem()
        {
            AssociatedTexts = new List<string>();    
        }
        
        public string Description { get; set; }
        public List<string> AssociatedTexts { get; set; }
    } 
}