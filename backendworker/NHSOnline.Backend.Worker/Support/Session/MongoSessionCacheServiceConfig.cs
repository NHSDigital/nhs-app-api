using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.Session
{
    public class MongoSessionCacheServiceConfig: IMongoSessionCacheServiceConfig
    {
        public string SessionMongoDatabaseHost { get; }
        public int SessionMongoDatabasePort { get; }  
        public string SessionMongoDatabaseUsername { get; }
        public string SessionMongoDatabasePassword { get; }
        
        public string SessionMongoDatabaseName  { get; }
        public string SessionMongoDatabaseCollection  { get; }

        public MongoSessionCacheServiceConfig(IConfiguration configuration,
            ILogger<MongoSessionCacheServiceConfig> logger)
        {
            SessionMongoDatabaseHost = configuration.GetOrThrow("SESSION_MONGO_DATABASE_HOST", logger);
            SessionMongoDatabasePort = configuration.GetIntOrThrow("SESSION_MONGO_DATABASE_PORT", logger);
            SessionMongoDatabaseUsername = configuration.GetOrNull("SESSION_MONGO_DATABASE_USERNAME");
            SessionMongoDatabasePassword = configuration.GetOrNull("SESSION_MONGO_DATABASE_PASSWORD");

            SessionMongoDatabaseName = configuration.GetOrThrow("SESSION_MONGO_DATABASE_NAME", logger);
            SessionMongoDatabaseCollection = configuration.GetOrThrow("SESSION_MONGO_DATABASE_COLLECTION", logger);
        }
    }
}