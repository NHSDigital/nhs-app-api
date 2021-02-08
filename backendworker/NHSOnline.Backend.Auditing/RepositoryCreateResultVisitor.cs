using System.Threading.Tasks;
using NHSOnline.Backend.Repository;

namespace NHSOnline.Backend.Auditing
{
    internal class RepositoryCreateResultVisitor : IRepositoryCreateResultVisitor<AuditRecord, Task>
    {
        private readonly AuditRecord _auditRecord;

        public RepositoryCreateResultVisitor(AuditRecord auditRecord)
        {
            _auditRecord = auditRecord;
        }

        public Task Visit(RepositoryCreateResult<AuditRecord>.Created result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(RepositoryCreateResult<AuditRecord>.RepositoryError result)
        {
            if (result.OriginalException != null)
            {
                throw new PreOperationAuditException(
                    $"Error writing PreAudit Operation: {_auditRecord.Operation}",
                    result.OriginalException);
            }
            throw new PreOperationAuditException($"Error writing PreAudit Operation: {_auditRecord.Operation}");
        }
    }
}

