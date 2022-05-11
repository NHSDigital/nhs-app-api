using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary
{
    public class SecondaryCareSummaryMetricEtl : AuditLogEtl<SecondaryCareSummaryMetric>
    {
        public SecondaryCareSummaryMetricEtl(IEventsRepository repo, IAuditLogParser<SecondaryCareSummaryMetric> parser)
            : base(repo, parser)
        {

        }

        protected override string StoredProcedureName =>
            "CALL events.SecondaryCareSummaryMetricInsert({0},{1},{2},{3},{4})";

        protected override object[] ReturnParams(SecondaryCareSummaryMetric metric)
        {
            return new object[]
            {
                metric.Timestamp,
                metric.SessionId,
                metric.TotalReferrals,
                metric.TotalUpcomingAppointments,
                metric.AuditId
            };
        }
    }
}