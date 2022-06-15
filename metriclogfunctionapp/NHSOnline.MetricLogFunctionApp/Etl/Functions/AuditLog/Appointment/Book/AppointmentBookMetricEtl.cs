using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Appointment.Book;

public class AppointmentBookMetricEtl : AuditLogEtl<AppointmentBookMetric>
{
    public AppointmentBookMetricEtl(IEventsRepository repo, IAuditLogParser<AppointmentBookMetric> parser, IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
        : base(repo, parser, queueOrchestrator)
    {
    }

    protected override string StoredProcedureName => "CALL events.AppointmentBookMetricInsert({0},{1},{2})";

    protected override object[] ReturnParams(AppointmentBookMetric metric)
    {
        return new object[]
        {
            metric.Timestamp,
            metric.SessionId,
            metric.AuditId,
        };
    }
}