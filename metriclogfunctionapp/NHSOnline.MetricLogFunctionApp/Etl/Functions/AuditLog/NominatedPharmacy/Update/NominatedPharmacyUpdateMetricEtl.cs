using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.NominatedPharmacy.Update
{
    public class NominatedPharmacyUpdateMetricEtl : AuditLogEtl<NominatedPharmacyUpdateMetric>
    {
        public NominatedPharmacyUpdateMetricEtl(IEventsRepository repo, IAuditLogParser<NominatedPharmacyUpdateMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator) : base(repo, parser, queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.NominatedPharmacyUpdateMetricInsert({0},{1},{2})";

        protected override object[] ReturnParams(NominatedPharmacyUpdateMetric metric)
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
