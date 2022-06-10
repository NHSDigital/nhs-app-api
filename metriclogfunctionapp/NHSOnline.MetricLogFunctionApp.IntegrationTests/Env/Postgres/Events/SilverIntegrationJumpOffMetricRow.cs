using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

public class SilverIntegrationJumpOffMetricRow : ITableRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string SessionId { get; set; }
    public string ProviderId { get; set; }
    public string ProviderName { get; set; }
    public string JumpOffId { get; set; }

    public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""SessionId"", ""ProviderId"", ""ProviderName"", ""JumpOffId"")
VALUES(@Timestamp, @SessionId , @ProviderId, @ProviderName, @JumpOffId)
";
}
