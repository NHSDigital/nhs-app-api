using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class InrResultData
    {
        public int Count { get; set; }
        
        [JsonProperty("data")]
        public List<InrResult> InrResults { get; set; } = new List<InrResult>(); 
    }
}