using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Journeys : ICloneable<Journeys>
    {
        public Appointments Appointments { get; set; }

        public Cdss CdssAdvice { get; set; }

        public Cdss CdssAdmin { get; set; }

        public Journeys Clone() => new Journeys
        {
            Appointments = Appointments?.Clone(),
            CdssAdvice = CdssAdvice?.Clone(),
            CdssAdmin = CdssAdmin?.Clone()
        };
    }
}