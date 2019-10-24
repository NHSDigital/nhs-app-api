using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.Auditing
{
    public class MongoDbAuditorSink : IAuditSink
    {
        private readonly IApiMongoClient<MongoDbAuditSinkConfiguration> _client;
        private readonly ILogger<MongoDbAuditorSink> _logger;
        private readonly string _databaseName;
        private readonly string _collectionName;

        public MongoDbAuditorSink(IApiMongoClient<MongoDbAuditSinkConfiguration> client,
            MongoDbAuditSinkConfiguration mongoConfiguration,
            ILogger<MongoDbAuditorSink> logger)
        {
            _client = client;
            _logger = logger;
            
            _databaseName = mongoConfiguration.DatabaseName;
            _collectionName = mongoConfiguration.CollectionName;
        }
        
        public async Task WriteAudit(AuditRecord auditRecord)
        {
            _logger.LogEnter();

            using (_logger.WithTimer("Add audit entry to MongoDB"))
            {
                await GetCollection().InsertOneAsync(auditRecord);
            }
            
            _logger.LogExit();
        }
        
        private IMongoCollection<AuditRecord> GetCollection()
            => _client.GetDatabase(_databaseName).GetCollection<AuditRecord>(_collectionName);
    }
}