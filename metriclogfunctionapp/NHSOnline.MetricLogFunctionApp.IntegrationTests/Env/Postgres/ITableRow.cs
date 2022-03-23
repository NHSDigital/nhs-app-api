namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal interface ITableRow
    {
        string InsertSql(string tableName);
    }
}