using MongoDB.Driver;

namespace NHSOnline.Backend.Repository
{
    public interface IMongoClientCreator
    {
        public INamedMongoClient CreatePrimaryMongoClient(string connectionString);

        public INamedMongoClient CreateSecondaryMongoClient(string connectionString);
    }

    public class MongoClientCreator : IMongoClientCreator
    {
        public INamedMongoClient CreatePrimaryMongoClient(string connectionString)
        {
            return CreateMongoClient(AzureMongoClientType.Primary, connectionString);
        }

        public INamedMongoClient CreateSecondaryMongoClient(string connectionString)
        {
            return CreateMongoClient(AzureMongoClientType.Secondary, connectionString);
        }

        private INamedMongoClient CreateMongoClient(AzureMongoClientType clientType, string connectionString)
        {
            var mongoUrl = new MongoUrl(connectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            return new NamedMongoClient(clientType, mongoClientSettings);
        }
    }

    public interface INamedMongoClient : IMongoClient
    {
        public AzureMongoClientType ClientType { get; }
    }

    public class NamedMongoClient : MongoClient, INamedMongoClient
    {
        public NamedMongoClient(AzureMongoClientType clientType, MongoClientSettings settings) : base(settings)
        {
            ClientType = clientType;
        }

        public AzureMongoClientType ClientType { get; private set; }
    }
}