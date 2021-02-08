using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Auditing
{
    public class DbAuditorSink : IAuditSink
    {
        private readonly IRepository<AuditRecord> _repository;
        private readonly ILogger<DbAuditorSink> _logger;

        public DbAuditorSink(
            IRepository<AuditRecord> repository,
            ILogger<DbAuditorSink> logger
        )
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task WritePreOperationAudit(AuditRecord auditRecord)
        {
            _logger.LogEnter();
            var result = await _repository.Create(auditRecord, nameof(AuditRecord));

            //make sure any pre-operation exceptions are thrown
            await result.Accept(new RepositoryCreateResultVisitor(auditRecord));
            _logger.LogExit();
        }

        public async Task WritePostOperationAudit(AuditRecord auditRecord)
        {
            _logger.LogEnter();
            await _repository.Create(auditRecord, nameof(AuditRecord));
            _logger.LogExit();
        }
    }
}
