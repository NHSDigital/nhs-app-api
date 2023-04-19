using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class AppointmentBookMetricRow : ITableRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }
        public string AuditId { get; set; }
        public bool IsActingOnBehalfOfAnother { get; set; }

        public string InsertSql(string tableName) => @$"
            INSERT INTO {tableName}(""Timestamp"", ""SessionId"", ""AuditId"", ""IsActingOnBehalfOfAnother"")
            VALUES(@Timestamp, @SessionId, @AuditId, @IsActingOnBehalfOfAnother)
            ";
    }
}