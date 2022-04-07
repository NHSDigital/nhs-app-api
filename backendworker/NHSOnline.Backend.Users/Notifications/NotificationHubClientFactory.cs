using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.Users.Notifications
{
    public class NotificationHubClientFactory : INotificationHubClientFactory
    {
        public INotificationHubClient CreateClientFromConnectionString(
            string connectionString,
            string notificationHubPath) =>
            NotificationHubClient.CreateClientFromConnectionString(connectionString, notificationHubPath);
    }
}