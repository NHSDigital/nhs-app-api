using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.Users.Notifications
{
    public interface INotificationHubClientFactory
    {
        INotificationHubClient CreateClientFromConnectionString(string connectionString, string notificationHubPath);
    }
}
