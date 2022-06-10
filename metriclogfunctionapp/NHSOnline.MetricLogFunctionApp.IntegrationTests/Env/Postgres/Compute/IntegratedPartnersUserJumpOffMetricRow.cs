using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

public class IntegratedPartnersUserJumpOffMetricRow : ITableRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string SessionId { get; set; }
    public string LoginId { get; set; }
    public string Provider { get; set; }
    public string JumpOff { get; set; }
    public string OdsCode { get; set; }
    public string Access { get; set; }
    public string Manufacturer { get; set; }
    public string Model { get; set; }
    public string Stp { get; set; }
    public string Ccg { get; set; }
    public string PracticeName { get; set; }
    public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""SessionId"", ""LoginId"", ""Provider"", ""JumpOff"", ""OdsCode"", ""Access"", ""Manufacturer"", ""Model"", ""Stp"", ""Ccg"", ""PracticeName"")
VALUES(@Timestamp, @SessionId, @LoginId, @Provider, @JumpOff, @OdsCode, @Access, @Manufacturer, @Model, @Stp, @Ccg, @PracticeName)
";
}
