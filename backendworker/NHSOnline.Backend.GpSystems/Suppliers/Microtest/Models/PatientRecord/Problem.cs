using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class Problem
    {
        [JsonProperty("start_date")]
        public string StartDate { get; set; }
        
        [JsonProperty("finish_date")]
        public string FinishDate { get; set; }
        
        public string Rubric { get; set; }    
    }
}