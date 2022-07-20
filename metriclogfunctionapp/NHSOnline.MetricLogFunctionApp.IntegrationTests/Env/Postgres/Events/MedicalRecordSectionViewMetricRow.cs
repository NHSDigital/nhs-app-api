using System;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

internal sealed class MedicalRecordSectionViewMetricRow : ITableRow
{
    public DateTimeOffset Timestamp { get; set; }
    public string SessionId { get; set; }
    public string Supplier  { get; set; }
    public bool IsActingOnBehalfOfAnother { get; set; }
    public string Section { get; set; }
    public string AuditId { get; set; }

    public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""Timestamp"", ""SessionId"", ""Supplier"", ""IsActingOnBehalfOfAnother"", ""Section"", ""AuditId"")
VALUES(@Timestamp, @SessionId, @Supplier, @IsActingOnBehalfOfAnother, @Section, @AuditId)
";
}