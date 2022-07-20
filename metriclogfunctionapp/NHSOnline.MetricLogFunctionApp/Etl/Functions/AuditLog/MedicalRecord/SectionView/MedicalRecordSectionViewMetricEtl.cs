using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.SectionView;

public class MedicalRecordSectionViewMetricEtl : AuditLogEtl<MedicalRecordSectionViewMetric>
{
    public MedicalRecordSectionViewMetricEtl(IEventsRepository repo, IAuditLogParser<MedicalRecordSectionViewMetric> parser,IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
        : base(repo, parser,queueOrchestrator)
    {

    }

    protected override string StoredProcedureName =>
        "CALL events.MedicalRecordSectionViewMetricInsert({0},{1},{2},{3},{4},{5})";

    protected override object[] ReturnParams(MedicalRecordSectionViewMetric metric)
    {
        return new object[]
        {
            metric.Timestamp,
            metric.SessionId,
            metric.Supplier,
            metric.IsActingOnBehalfOfAnother,
            metric.Section,
            metric.AuditId
        };
    }
}
