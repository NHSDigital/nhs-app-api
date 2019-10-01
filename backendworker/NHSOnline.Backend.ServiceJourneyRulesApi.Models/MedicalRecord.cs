using Newtonsoft.Json;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class MedicalRecord : Journey<MedicalRecordProvider>, ICloneable<MedicalRecord>
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public int Version { get; set; }
        
        public MedicalRecord Clone() => MemberwiseClone() as MedicalRecord;
    }
}