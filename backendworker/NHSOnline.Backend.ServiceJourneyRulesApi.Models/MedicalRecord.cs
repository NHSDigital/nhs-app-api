using Newtonsoft.Json;
using NHSOnline.Backend.Support;
using YamlDotNet.Core;
using YamlDotNet.Serialization;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class MedicalRecord : Journey<MedicalRecordProvider>, ICloneable<MedicalRecord>
    {
        [JsonProperty(NullValueHandling = NullValueHandling.Ignore)]
        [YamlMember(ScalarStyle = ScalarStyle.DoubleQuoted)]
        public string Version { get; set; }

        public MedicalRecord Clone() => MemberwiseClone() as MedicalRecord;
    }
}