using System;
namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class InitialPromptMetricRow : ITableRow
    {
        public string LoginId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string OptedIn { get; set; }
        public string AuditId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""LoginId"", ""Timestamp"", ""OptedIn"", ""AuditId"")
VALUES(@LoginId, @Timestamp, @OptedIn, @AuditId)
";
    }
}