using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Support.Auditing
{
    public interface IAuditSink
    {
        Task WriteAudit(AuditRecord auditRecord);
    }
}
