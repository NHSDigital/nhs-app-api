using Newtonsoft.Json;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Prescriptions : Journey<PrescriptionsProvider>, ICloneable<Prescriptions>
    {
        public Prescriptions Clone() => MemberwiseClone() as Prescriptions;
    }
}