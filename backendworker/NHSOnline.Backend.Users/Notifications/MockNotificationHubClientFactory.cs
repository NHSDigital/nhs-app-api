using Microsoft.Azure.NotificationHubs;

namespace NHSOnline.Backend.Users.Notifications
{
    public class MockNotificationHubClientFactory : INotificationHubClientFactory
    {
        private const int SleepTimeInMilliSeconds = 0;
        public INotificationHubClient CreateClientFromConnectionString(
            string connectionString,
            string notificationHubPath) =>
            new MockNotificationHubClient(SleepTimeInMilliSeconds);
    }
}
