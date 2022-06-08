using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Notification.InitialPrompt
{
    public class InitialPromptMetricEtl : AuditLogEtl<InitialPromptMetric>
    {
        public InitialPromptMetricEtl(IEventsRepository repo, IAuditLogParser<InitialPromptMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser, queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.InitialPromptMetricInsert({0},{1},{2},{3})";

        protected override object[] ReturnParams(InitialPromptMetric metric)
        {
            return new object[]
            {
                metric.LoginId,
                metric.Timestamp,
                metric.OptedIn,
                metric.AuditId
            };
        }
    }
}