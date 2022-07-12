using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Create
{
    public class NominatedPharmacyCreateMetricEtl : AuditLogEtl<NominatedPharmacyCreateMetric>
    {
        public NominatedPharmacyCreateMetricEtl(IEventsRepository repo, IAuditLogParser<NominatedPharmacyCreateMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator) : base(repo, parser, queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.NominatedPharmacyCreateMetricInsert({0},{1},{2})";

        protected override object[] ReturnParams(NominatedPharmacyCreateMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.SessionId,
                metric.AuditId
            };
        }
    }
}
