using System;
namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class NotificationToggleMetricRow : ITableRow
    {
        public string LoginId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string NotificationToggle { get; set; }
        public string AuditId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""LoginId"", ""Timestamp"", ""NotificationToggle"", ""AuditId"")
VALUES(@LoginId, @Timestamp, @NotificationToggle, @AuditId)
";
    }
}