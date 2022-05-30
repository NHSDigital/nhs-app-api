using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class NominatedPharmacyUpdateMetricRow : ITableRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""SessionId"")
VALUES(@Timestamp, @SessionId)
";
    }
}