using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

internal sealed class LastLoginPatientIdentifierRow : ITableRow
{
    public string AuditId { get; set; }
    public string LoginId { get; set; }
    public string NhsNumber { get; set; }
    public DateTimeOffset Timestamp { get; set; }

    public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""LoginId"", ""NhsNumber"", ""Timestamp"", ""AuditId"")
VALUES(@LoginId, @NhsNumber,  @Timestamp, @AuditId)
";
}
