namespace NHSOnline.Backend.UsersApi
{
    internal class AzureNotificationConfiguration
    {
        public string ConnectionString { get; }
        public string NotificationHubPath { get; }
        public string SharedAccessKey { get; }
        
        public AzureNotificationConfiguration(string connectionString, string notificationHubPath, string sharedAccessKey)
        {
            ConnectionString = connectionString;
            NotificationHubPath = notificationHubPath;
            SharedAccessKey = sharedAccessKey;
        }
    }
}