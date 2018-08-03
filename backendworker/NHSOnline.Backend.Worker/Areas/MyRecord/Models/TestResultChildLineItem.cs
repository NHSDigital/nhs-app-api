using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class TestResultChildLineItem
    {
        public string Description { get; set; }
        public List<string> AssociatedTexts { get; set;  }
        
        public TestResultChildLineItem()
        {
            AssociatedTexts = new List<string>();    
        }
    } 
}