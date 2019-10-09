using System.Collections.Generic;
using Newtonsoft.Json;

namespace NHSOnline.Backend.GpSystems.Suppliers.Microtest.Models.PatientRecord
{
    public class ReferralData
    {
        public bool HasAccess { get; set; }
         
        public bool HasErrored { get; set; }
         
        public int Count { get; set; }
         
        [JsonProperty("data")]
        public List<Referral> Referrals { get; set; } = new List<Referral>();   
    }
}