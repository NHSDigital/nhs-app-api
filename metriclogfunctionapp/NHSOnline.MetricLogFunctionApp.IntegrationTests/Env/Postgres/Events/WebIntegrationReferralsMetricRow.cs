using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class WebIntegrationReferralsMetricRow : ITableRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string Referrer { get; set; }
        public string ReferrerOrigin { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""Referrer"", ""ReferrerOrigin"", ""SessionId"", ""AuditId"" )
VALUES(@Timestamp, @Referrer, @ReferrerOrigin, @SessionId, @AuditId)
";
    }
}