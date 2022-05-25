using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView
{
    public class MedicalRecordViewMetricEtl : AuditLogEtl<MedicalRecordViewMetric>
    {
        public MedicalRecordViewMetricEtl(IEventsRepository repo, IAuditLogParser<MedicalRecordViewMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
            : base(repo, parser,queueOrchestrator)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.MedicalRecordViewMetricInsert({0},{1},{2},{3},{4})";

        protected override object[] ReturnParams(MedicalRecordViewMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.SessionId,
                metric.HasSummaryRecordAccess,
                metric.HasDetailedRecordAccess,
                metric.AuditId
            };
        }
    }
}