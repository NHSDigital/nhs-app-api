namespace NHSOnline.Backend.UsersApi
{
    internal class MongoConfiguration : IMongoConfiguration
    {
        public string DatabaseName { get; }
        public string UserDeviceCollectionName { get; }
        public string Host { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }

        public MongoConfiguration(string host, int port, string databaseName, string username, string password,
            string userDeviceCollectionName)
        {
            DatabaseName = databaseName;
            UserDeviceCollectionName = userDeviceCollectionName;
            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }
    }
}