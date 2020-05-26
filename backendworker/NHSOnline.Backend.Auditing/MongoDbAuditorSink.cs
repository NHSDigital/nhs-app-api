using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using MongoDB.Driver;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Repository;

namespace NHSOnline.Backend.Auditing
{
    public class MongoDbAuditorSink : MongoRepository<MongoDbAuditSinkConfiguration, AuditRecord>, IAuditSink
    {
        private readonly ILogger<MongoDbAuditorSink> _logger;

        public MongoDbAuditorSink(
            IApiMongoClient<MongoDbAuditSinkConfiguration> client,
            MongoDbAuditSinkConfiguration mongoConfiguration,
            ILogger<MongoDbAuditorSink> logger
        ) : base(client, mongoConfiguration)
        {
            _logger = logger;
        }

        public async Task WriteAudit(AuditRecord auditRecord)
        {
            _logger.LogEnter();

            using (_logger.WithTimer("Add audit entry to MongoDB"))
            {
                await InsertOne(auditRecord);
            }

            _logger.LogExit();
        }
    }
}