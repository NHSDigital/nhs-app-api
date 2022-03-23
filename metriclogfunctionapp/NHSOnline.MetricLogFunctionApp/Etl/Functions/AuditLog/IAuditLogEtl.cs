using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
{
    public interface IAuditLogEtl<T>
    {
        public Task Execute(IList<AuditRecord> events);
    }
}
