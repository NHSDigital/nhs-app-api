using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
{
    public abstract class AuditLogEtl<T> : IAuditLogEtl<T> where T : IEventRepositoryRow
    {
        private readonly IEventsRepository _repo;
        private readonly IAuditLogParser<T> _parser;
        protected abstract string StoredProcedureName { get; }

        protected AuditLogEtl(IEventsRepository repo, IAuditLogParser<T> parser)
        {
            _repo = repo;
            _parser = parser;
        }

        public async Task Execute(IList<AuditRecord> events)
        {
            var metrics = events.Select(x => _parser.Parse(x))
                .Where(x => x != null)
                .ToList();

            foreach (var metric in metrics)
            {
                await _repo.CallStoredProcedure(StoredProcedureName, ReturnParams(metric));
            }
        }

        protected abstract object[] ReturnParams(T metric);
    }
}
