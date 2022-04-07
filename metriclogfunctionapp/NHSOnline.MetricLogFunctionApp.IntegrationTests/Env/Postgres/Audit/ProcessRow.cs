using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Audit
{
    internal sealed class ProcessRow : ITableRow
    {
        public int Id { get; set; }
        public bool IsSuccess { get; set; }
        public string ProcessName { get; set; }
        public DateTime RangeStartDateTime { get; set; }
        public DateTime RangeEndDateTime { get; set; }
        public DateTime ProcessStartDateTime { get; set; }
        public DateTime ProcessEndDateTime { get; set; }
        public TimeSpan Duration { get; set; }
        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""IsSuccess"", ""ProcessName"", ""RangeStartDateTime"", ""RangeEndDateTime"", ""ProcessStartDateTime"", ""ProcessEndDateTime"", ""Duration"")
VALUES(@IsSuccess, @ProcessName, @RangeStartDateTime, @RangeEndDateTime, @ProcessStartDateTime, @ProcessEndDateTime, @Duration)
";
    }
}