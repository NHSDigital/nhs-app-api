using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public interface IAuditSink
    {
        void WriteAudit(DateTime timestamp, string nhsNumber, string supplier, string operation, string details);
    }
}
