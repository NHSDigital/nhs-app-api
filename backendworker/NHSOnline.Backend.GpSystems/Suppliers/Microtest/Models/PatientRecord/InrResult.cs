using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class InrResult
    {
        public string RecordDateTime { get; set; }
        
        [JsonProperty("codeDescr")]
        public string CodeDescription { get; set; }
        
        public string Therapy { get; set; }
        
        public string Target { get; set; }
        
        public string Value { get; set; }
        
        public string Dose { get; set; }

        public string NextTestDate { get; set; }
    }
}