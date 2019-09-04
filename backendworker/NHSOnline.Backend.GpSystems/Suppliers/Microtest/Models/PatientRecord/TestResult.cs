using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class TestResult
    {
        [JsonProperty("inrResults")]
        public InrResultData InrResultsData { get; set; }
        
        [JsonProperty("pathResults")]
        public PathResultData PathResultsData { get; set; }
    }
}