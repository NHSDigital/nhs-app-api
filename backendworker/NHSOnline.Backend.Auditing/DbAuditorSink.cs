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

        public async Task WriteAudit(AuditRecord auditRecord)
        {
            _logger.LogEnter();
            await _repository.Create(auditRecord, nameof(AuditRecord));

            _logger.LogExit();
        }
    }
}