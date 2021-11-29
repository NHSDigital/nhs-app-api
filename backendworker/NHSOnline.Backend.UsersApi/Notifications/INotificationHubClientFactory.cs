using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public interface INotificationHubClientFactory
    {
        INotificationHubClient CreateClientFromConnectionString(string connectionString, string notificationHubPath);
    }
}
