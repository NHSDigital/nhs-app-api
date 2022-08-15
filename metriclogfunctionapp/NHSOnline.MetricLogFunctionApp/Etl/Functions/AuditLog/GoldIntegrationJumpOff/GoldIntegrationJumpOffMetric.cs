using System;
using System.ComponentModel.DataAnnotations.Schema;
using NHSOnline.MetricLogFunctionApp.Etl.Load;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.GoldIntegrationJumpOff;

[Table("GoldIntegrationJumpOffMetric", Schema = "events")]
public class GoldIntegrationJumpOffMetric : IEventRepositoryRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string SessionId { get; set; }
    public string ProviderId { get; set; }
    public string ProviderName { get; set; }
    public string JumpOffId { get; set; }
    public string AuditId { get; set; }
}
