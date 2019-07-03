using Newtonsoft.Json;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class MedicalRecord : Journey<MedicalRecordProvider>, ICloneable<MedicalRecord>
    {
        public MedicalRecord Clone() => MemberwiseClone() as MedicalRecord;
    }
}