namespace NHSOnline.Backend.Users.Notifications
{
    public class NotificationsConfiguration : INotificationsConfiguration
    {
        public bool IosBadgeCountEnabled { get; }
        public int NotificationInstallationExpiryMonths { get; }

        public NotificationsConfiguration(bool iosBadgeCountEnabled, int notificationInstallationExpiryMonths)
        {
            IosBadgeCountEnabled = iosBadgeCountEnabled;
            NotificationInstallationExpiryMonths = notificationInstallationExpiryMonths;
        }

    }
}