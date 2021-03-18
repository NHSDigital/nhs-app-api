namespace NHSOnline.App.DependencyServices.Notifications
{
    public interface INotifications
    {
        NotificationStatus GetDeviceNotificationsStatus();
        GetPnsTokenResult GetPnsToken();
    }
}