using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class MedicalHistoryData
    {
        public bool HasAccess { get; set; }
        
        public bool HasErrored { get; set; }
        
        public int Count { get; set; }
        
        [JsonProperty("data")]
        public List<MedicalHistory> MedicalHistories { get; set; } = new List<MedicalHistory>();
    }
}