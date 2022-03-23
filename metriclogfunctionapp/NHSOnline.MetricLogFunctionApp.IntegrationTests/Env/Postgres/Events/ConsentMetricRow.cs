using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class ConsentMetricRow : ITableRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string OdsCode { get; set; }
        public string LoginId { get; set; }
        public string ProofLevel { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""OdsCode"", ""LoginId"", ""ProofLevel"", ""SessionId"", ""AuditId"")
VALUES(@Timestamp, @OdsCode, @LoginId, @ProofLevel, @SessionId, @AuditId)
";
    }
}