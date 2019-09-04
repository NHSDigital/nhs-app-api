using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class PathResultData
    {
        public int Count { get; set; }
        
        [JsonProperty("data")]
        public List<PathResult> PathResults { get; set; } = new List<PathResult>(); 
    }
}