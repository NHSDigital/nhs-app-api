using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

internal sealed class GoldIntegrationJumpOffMetricRow : ITableRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string SessionId { get; set; }
    public string ProviderId { get; set; }
    public string ProviderName { get; set; }
    public string JumpOffId { get; set; }
    public string AuditId { get; set; }

    public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""SessionId"", ""ProviderId"", ""ProviderName"", ""JumpOffId"", ""AuditId"")
VALUES(@Timestamp, @SessionId , @ProviderId, @ProviderName, @JumpOffId, @AuditId)
";
}
