using System.Threading.Tasks;

namespace NHSOnline.Backend.Auditing
{
    public interface IAuditSink
    {
        Task WriteAudit(AuditRecord auditRecord);
    }
}
