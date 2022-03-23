using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class WebIntegrationReferralsMetricRow : ITableRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Referrer { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""Referrer"", ""SessionId"", ""AuditId"")
VALUES(@Timestamp, @Referrer, @SessionId, @AuditId)
";
    }
}