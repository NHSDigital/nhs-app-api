using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
{
    public abstract class AuditLogEtl<T> : IAuditLogEtl<T> where T : IEventRepositoryRow
    {
        private readonly IEventsRepository _repo;
        private readonly IAuditLogParser<T> _parser;
        protected abstract string StoredProcedureName { get; }
        private readonly IRequestQueueOrchestrator<AuditReportRequest> _queueOrchestrator;

        protected AuditLogEtl(IEventsRepository repo, IAuditLogParser<T> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
        {
            _repo = repo;
            _parser = parser;
            _queueOrchestrator = queueOrchestrator;
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

        public async Task ExecuteDependentEvent(ILogger logger, IList<AuditRecord> events)
        {
            var metrics = events.Select(x => _parser.Parse(x))
                .Where(x => x != null)
                .ToList();

            foreach (var metric in metrics)
            {
                await _repo.CallStoredProcedure(StoredProcedureName, ReturnParams(metric));
                await OrchestrateDependentEvents(metric, logger);
            }
        }

        private async Task OrchestrateDependentEvents(T metric, ILogger logger)
        {
            string dependentQueue = null;
            AuditReportRequest message = null;
            switch (metric)
            {
                case ConsentMetric consent:
                    message = new AuditReportRequest
                    {
                        LoginId = consent.LoginId,
                        StartDateTime = consent.Timestamp.DateTime.AddSeconds(-1),
                        EndDateTime = consent.Timestamp.DateTime.AddSeconds(1)
                    };
                    dependentQueue = "first-logins-metric-";
                    break;
                case LoginMetric login:
                    message = new AuditReportRequest
                    {
                        LoginId = login.LoginId,
                        StartDateTime = login.Timestamp.DateTime.AddSeconds(-1),
                        EndDateTime = login.Timestamp.DateTime.AddSeconds(1)
                    };
                    dependentQueue = "first-logins-metric-";
                    break;

            }
            await _queueOrchestrator.AddMessage(logger, dependentQueue,message);
        }

        protected abstract object[] ReturnParams(T metric);
    }
}
