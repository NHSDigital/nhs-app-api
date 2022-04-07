using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Audit
{
    internal sealed class ProcessDurationRow : ITableRow
    {
        public int Id { get; set; }
        public string ProcessName { get; set; }
        public DateTime StartDateTime { get; set; }
        public DateTime EndDateTime { get; set; }
        public int Duration { get; set; }
        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""ProcessName"", ""StartDateTime"", ""EndDateTime"", ""Duration"")
VALUES(@ProcessName, @StartDateTime, @EndDateTime, @Duration)
";
    }
}