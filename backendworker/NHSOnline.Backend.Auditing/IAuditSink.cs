using System.Threading.Tasks;

namespace NHSOnline.Backend.Auditing
{
    public interface IAuditSink
    {
        Task WritePreOperationAudit(AuditRecord auditRecord);

        Task WritePostOperationAudit(AuditRecord auditRecord);
    }
}
