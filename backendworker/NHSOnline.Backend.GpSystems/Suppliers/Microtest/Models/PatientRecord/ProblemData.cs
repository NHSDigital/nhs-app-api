using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class ProblemData
    {
         public bool HasAccess { get; set; }
         
         public bool HasErrored { get; set; }
         
         public int Count { get; set; }
         
         [JsonProperty("data")]
         public List<Problem> Problems { get; set; } = new List<Problem>();       
    }
}