namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

internal sealed class TestOdsCodesRow : ITableRow
{
    public string OdsCode { get; set; }

    public string InsertSql(string tableName) => @$"
INSERT INTO {tableName}(""OdsCode"")
VALUES(@OdsCode)
";
}
