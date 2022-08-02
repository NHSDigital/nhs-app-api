using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Device
{
    [Table("Device", Schema = "events")]
    public class DeviceMetric : IEventRepositoryRow
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
    }
}
