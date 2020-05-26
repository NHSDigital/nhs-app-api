using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.Auditing
{
    public class MongoDbAuditSinkConfiguration : IMongoConfiguration
    {
        public string ConnectionString { get; }
        public string DatabaseName { get; }
        public string CollectionName { get; }

        public MongoDbAuditSinkConfiguration(IConfiguration configuration, ILogger<MongoDbAuditSinkConfiguration> logger)
        {
            ConnectionString = configuration.GetOrThrow("AUDIT_MONGO_CONNECTION_STRING", logger);
            DatabaseName = configuration.GetOrThrow("AUDIT_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("AUDIT_MONGO_DATABASE_COLLECTION", logger);
        }
    }
}