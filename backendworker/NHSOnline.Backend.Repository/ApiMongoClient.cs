using MongoDB.Driver;

namespace NHSOnline.Backend.Repository
{
    public class ApiMongoClient<TConfiguration> : MongoClient, IApiMongoClient<TConfiguration> where TConfiguration : IMongoConfiguration
    {
        public ApiMongoClient(TConfiguration mongoConfiguration)
            : base(BuildSettings(mongoConfiguration))
        {
        }

        private static MongoClientSettings BuildSettings(TConfiguration mongoConfiguration)
        {
            var mongoUrl = new MongoUrl(mongoConfiguration.ConnectionString);
            var mongoClientSettings = MongoClientSettings.FromUrl(mongoUrl);
            return mongoClientSettings;
        }
    }
}