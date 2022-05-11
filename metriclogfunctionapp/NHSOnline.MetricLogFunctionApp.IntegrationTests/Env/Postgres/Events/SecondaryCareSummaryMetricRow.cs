using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class SecondaryCareSummaryMetricRow : ITableRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }
        public int TotalReferrals { get; set; }
        public int TotalUpcomingAppointments { get; set; }
        public string AuditId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""SessionId"", ""TotalReferrals"", ""TotalUpcomingAppointments"", ""AuditId"")
VALUES(@Timestamp, @SessionId, @TotalReferrals, @TotalUpcomingAppointments, @AuditId)
";
    }
}