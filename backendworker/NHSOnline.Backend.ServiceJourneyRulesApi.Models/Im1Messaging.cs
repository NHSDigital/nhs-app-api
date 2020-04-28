using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Models
{
    public class Im1Messaging: ICloneable<Im1Messaging>
    {
        public bool? IsEnabled { set; get; }

        public bool? CanDeleteMessages { set; get; }

        public bool? CanUpdateReadStatus { set; get; }

        public bool? RequiresDetailsRequest { set; get; }

        public bool? SendMessageSubject { set; get; }

        public Im1Messaging Clone() => MemberwiseClone() as Im1Messaging;

        public void Merge(Im1Messaging other)
        {
            if (other?.IsEnabled != null)
            {
                IsEnabled = other.IsEnabled;
            }

            if (other?.CanDeleteMessages != null)
            {
                CanDeleteMessages = other.CanDeleteMessages;
            }

            if (other?.CanUpdateReadStatus != null)
            {
                CanUpdateReadStatus = other.CanUpdateReadStatus;
            }

            if (other?.RequiresDetailsRequest != null)
            {
                RequiresDetailsRequest = other.RequiresDetailsRequest;
            }

            if (other?.SendMessageSubject != null)
            {
                SendMessageSubject = other.SendMessageSubject;
            }
        }
    }
}