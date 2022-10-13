namespace NHSOnline.Backend.Users.Notifications
{
    public interface INotificationsConfiguration
    {
        bool IosBadgeCountEnabled { get; }
        int NotificationInstallationExpiryMonths { get; }
    }
}