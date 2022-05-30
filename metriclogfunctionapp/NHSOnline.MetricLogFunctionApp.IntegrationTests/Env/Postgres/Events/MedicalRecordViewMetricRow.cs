using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class MedicalRecordViewMetricRow : ITableRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }
        public bool HasSummaryRecordAccess { get; set; }
        public bool HasDetailedRecordAccess { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""SessionId"", ""HasSummaryRecordAccess"", ""HasDetailedRecordAccess"")
VALUES(@Timestamp, @SessionId, @HasSummaryRecordAccess, @HasDetailedRecordAccess)
";
    }
}