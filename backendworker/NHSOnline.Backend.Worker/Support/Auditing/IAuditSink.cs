using System;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public interface IAuditSink
    {
        void WriteAudit(DateTime timestamp, string nhsNumber, SupplierEnum supplier, string operation, string details);
    }
}
