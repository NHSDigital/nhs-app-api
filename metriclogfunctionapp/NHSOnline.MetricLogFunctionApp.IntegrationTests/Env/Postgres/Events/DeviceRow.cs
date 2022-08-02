using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events
{
    internal sealed class DeviceRow : ITableRow
    {
        public DateTimeOffset Timestamp { get; set; }
        public string SessionId { get; set; }
        public string AppVersion { get; set; }
        public string DeviceManufacturer { get; set; }
        public string DeviceModel { get; set; }
        public string DeviceOS { get; set; }
        public string DeviceOSVersion { get; set; }
        public string UserAgent { get; set; }
        public string AuditId { get; set; }

        public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""SessionId"", ""AppVersion"", ""DeviceManufacturer"", ""DeviceModel"", ""DeviceOS"", ""DeviceOSVersion"", ""UserAgent"", ""AuditId"")
VALUES(@Timestamp, @SessionId, @AppVersion, @DeviceManufacturer, @DeviceModel, @DeviceOS, @DeviceOSVersion, @UserAgent, @AuditId)
";
    }
}
