using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.BiometricsToggle;

[Table("BiometricsToggleMetric", Schema = "events")]
public class BiometricsToggleMetric : IEventRepositoryRow

{
    public string SessionId { get; set; }
    public DateTimeOffset Timestamp { get; set; }
    public string BiometricsToggle { get; set; }
    public string AuditId { get; set; }
}
