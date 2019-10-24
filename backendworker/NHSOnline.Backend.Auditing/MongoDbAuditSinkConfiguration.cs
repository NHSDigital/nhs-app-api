using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.Auditing
{
    public class MongoDbAuditSinkConfiguration : IMongoConfiguration
    {
        public string DatabaseName { get; }
        public string CollectionName { get; }
        public string Host { get; }
        public int Port { get; }
        public string Username { get; }
        public string Password { get; }
        
        public MongoDbAuditSinkConfiguration(IConfiguration configuration, ILogger<MongoDbAuditSinkConfiguration> logger)
        {
            DatabaseName = configuration.GetOrThrow("AUDIT_MONGO_DATABASE_NAME", logger);
            CollectionName = configuration.GetOrThrow("AUDIT_MONGO_DATABASE_COLLECTION", logger);
            Host = configuration.GetOrThrow("AUDIT_MONGO_DATABASE_HOST", logger);
            Port = configuration.GetIntOrThrow("AUDIT_MONGO_DATABASE_PORT", logger);
            Username = configuration.GetOrNull("AUDIT_MONGO_DATABASE_USERNAME");
            Password = configuration.GetOrNull("AUDIT_MONGO_DATABASE_PASSWORD");
        }
    }
}