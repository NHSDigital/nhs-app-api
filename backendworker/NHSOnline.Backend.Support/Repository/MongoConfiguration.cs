namespace NHSOnline.Backend.Support.Repository
{
    public class MongoConfiguration : IMongoConfiguration
    {
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string Host { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }

        public MongoConfiguration(string host, int port, string databaseName, string username, string password,
            string collectionName)
        {
            DatabaseName = databaseName;
            CollectionName = collectionName;
            Host = host;
            Port = port;
            Username = username;
            Password = password;
        }
    }
}