using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class Allergy
    {
        public int Id { get; set; }
        
        public string Type { get; set; }
        
        [JsonProperty("start_date")]
        public string StartDate { get; set; }
        
        public string Description { get; set; }
        
        public string Severity { get; set; }
    }
}