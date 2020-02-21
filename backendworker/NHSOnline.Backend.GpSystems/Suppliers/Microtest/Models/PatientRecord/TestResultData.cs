using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class TestResultData
    {
        public bool HasAccess { get; set; }
         
        public bool HasErrored { get; set; }
         
        public int Count { get; set; }
         
        [JsonProperty("data")]
        public TestResult TestResult { get; set; } = new TestResult();     
    }
}