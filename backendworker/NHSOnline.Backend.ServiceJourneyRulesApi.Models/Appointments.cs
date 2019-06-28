using Newtonsoft.Json;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Appointments : Journey<AppointmentsProvider>, ICloneable<Appointments>
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        public string InformaticaUrl { get; set; }

        public Appointments Clone() => MemberwiseClone() as Appointments;
    }
}