using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Im1Messaging: ICloneable<Im1Messaging>
    {
        public bool? IsEnabled { set; get; }

        public bool? CanDeleteMessages { set; get; }

        public Im1Messaging Clone() => MemberwiseClone() as Im1Messaging;
    }
}