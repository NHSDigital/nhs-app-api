using System.Diagnostics.CodeAnalysis;
using Newtonsoft.Json;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Appointments : Journey<AppointmentsProvider>, ICloneable<Appointments>
    {
        [JsonProperty(NullValueHandling=NullValueHandling.Ignore)]
        [SuppressMessage("Microsoft.Design", "CA1056", Justification = "Intentional; we wish to expose this as a string, do not intend to parse the URL")]
        public string InformaticaUrl { get; set; }

        public Appointments Clone() => MemberwiseClone() as Appointments;
    }
}