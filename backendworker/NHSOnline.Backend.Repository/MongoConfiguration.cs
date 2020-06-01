namespace NHSOnline.Backend.Repository
{
    public class MongoConfiguration : IMongoConfiguration
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }

        public MongoConfiguration(string connectionString, string databaseName, string collectionName)
        {
            ConnectionString = connectionString;
            DatabaseName = databaseName;
            CollectionName = collectionName;
        }
    }
}