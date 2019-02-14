using System;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Support.Auditing
{
    public interface IAuditSink
    {
        Task WriteAudit(DateTime timestamp, string nhsNumber, Supplier supplier, string operation, string details, VersionTag versionTag);
    }
}
