using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
{
    public interface IAuditLogEtl<T>
    {
        public Task Execute(IList<AuditRecord> events);
        public Task ExecuteDependentEvent(ILogger logger, IList<AuditRecord> events);
    }
}
