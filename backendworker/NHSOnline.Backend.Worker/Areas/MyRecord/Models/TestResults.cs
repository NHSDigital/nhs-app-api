using System.Collections.Generic;

namespace NHSOnline.Backend.Worker.Areas.MyRecord.Models
{
    public class TestResults : IPatientDataModel
    {
        public TestResults()
        {
            Data = new List<TestResultItem>();
            HasAccess = true;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public IEnumerable<TestResultItem> Data { get; set; }
        public string RawHtml { get; set; }
    }
}
