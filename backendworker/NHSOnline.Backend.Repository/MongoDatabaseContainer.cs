using MongoDB.Driver;

namespace NHSOnline.Backend.Repository
{
    public class MongoDatabaseContainer
    {
        public AzureMongoClientType ClientType { get; set; }
        public IMongoDatabase MongoDatabase { get; set; }
    }
}