using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Wayfinder: ICloneable<Wayfinder>
    {
        public bool? IsEnabled { set; get; }

        public Wayfinder Clone() => MemberwiseClone() as Wayfinder;
    }
}