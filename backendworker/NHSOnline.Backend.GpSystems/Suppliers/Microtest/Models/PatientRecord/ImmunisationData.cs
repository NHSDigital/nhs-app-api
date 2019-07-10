using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class ImmunisationData
    {
        public bool HasAccess { get; set; }
        
        public bool HasErrored { get; set; }
        
        public int Count { get; set; }
        
        [JsonProperty("data")]
        public List<Immunisation> Immunisations { get; set; } = new List<Immunisation>();
    }
}