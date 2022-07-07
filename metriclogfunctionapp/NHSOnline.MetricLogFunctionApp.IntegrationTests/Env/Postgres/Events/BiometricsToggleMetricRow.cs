using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class BiometricsToggleMetricRow : ITableRow
    {
        public string SessionId { get; set; }
        public DateTimeOffset Timestamp { get; set; }
        public string BiometricsToggle { get; set; }
        public string AuditId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""SessionId"", ""Timestamp"", ""BiometricsToggle"", ""AuditId"")
VALUES(@SessionId, @Timestamp, @BiometricsToggle, @AuditId)
";
    }
}
