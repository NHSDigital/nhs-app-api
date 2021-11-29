using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.UsersApi.Notifications
{
    public class MockNotificationHubClientFactory : INotificationHubClientFactory
    {
        private const int SleepTimeInMilliSeconds = 10;
        public INotificationHubClient CreateClientFromConnectionString(
            string connectionString,
            string notificationHubPath) =>
            new MockNotificationHubClient(SleepTimeInMilliSeconds);
    }
}
