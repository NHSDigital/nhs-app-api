namespace NHSOnline.Backend.Users.Areas.Devices.Models
{
    public class NotificationsAuditData
    {
        public bool NotificationsRegistered { get; }
        public NotificationsDecisionSource NotificationsDecisionSource { get; }

        public NotificationsAuditData(bool notificationsRegistered, NotificationsDecisionSource notificationsDecisionSource)
        {
            NotificationsRegistered = notificationsRegistered;
            NotificationsDecisionSource = notificationsDecisionSource;
        }
    }
}
