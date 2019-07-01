using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class Medication
    {
        public int Id { get; set; }
        
        public string Name { get; set; }
        
        public string Quantity { get; set; }
        
        public string Dosage { get; set; } 
        
        public string Status { get; set; }
        
        public string Type { get; set; }
        
        [JsonProperty("prescribed_date")]
        public string PrescribedDate { get; set; }
        
        [JsonProperty("first_prescribed_date")]
        public string FirstPrescribedDate { get; set; }
        
        public string Reason { get; set; }
    }
}