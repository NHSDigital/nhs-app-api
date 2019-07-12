using System.Collections.Generic;
using System.Linq;

namespace NHSOnline.Backend.GpSystems.PatientRecord.Models
{
    public class TestResults : IVisionPatientDataModel
    {
        public TestResults()
        {
            Data = new List<TestResultItem>();
            HasAccess = true;
            HasErrored = false;
        }
        
        public bool HasAccess { get; set; }
        public bool HasErrored { get; set; }
        public int RecordCount => Data?.Count() ?? 0;
        public IEnumerable<TestResultItem> Data { get; set; }
        public string RawHtml { get; set; }
    }
}
