namespace NHSOnline.Backend.UsersApi
{
    internal class AzureNotificationHubConfiguration
    {
        public string ConnectionString { get; }
        public string NotificationHubPath { get; }
        public string SharedAccessKey { get; }
        public string ReadCharacters { get; } 
        public string WriteCharacters { get; }
        public int Generation { get; }
        
        public AzureNotificationHubConfiguration(
            string connectionString,
            string notificationHubPath,
            string sharedAccessKey,
            string readCharacters,
            string writeCharacters,
            int generation
        )
        {
            ConnectionString = connectionString;
            NotificationHubPath = notificationHubPath;
            SharedAccessKey = sharedAccessKey;
            ReadCharacters = readCharacters.ToUpperInvariant();
            WriteCharacters = writeCharacters.ToUpperInvariant();
            Generation = generation;
        }
    }
}