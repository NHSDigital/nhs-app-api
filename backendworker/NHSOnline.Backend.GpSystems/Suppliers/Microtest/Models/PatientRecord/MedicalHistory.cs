using Newtonsoft.Json;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class MedicalHistory
    {     
        [JsonProperty("start_date")]
        public string StartDate { get; set; }
        
        public string Rubric { get; set; }
        
        public string Description { get; set; }
    }
}