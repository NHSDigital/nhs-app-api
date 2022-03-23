using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class PostgresTable<TRow> where TRow: ITableRow
    {
        private readonly PostgresWrapper _postgres;
        private readonly string _schemaName;
        private readonly string _tableName;

        internal PostgresTable(PostgresWrapper postgres, string tableName) : this(postgres, "events", tableName)
        {}

        internal PostgresTable(PostgresWrapper postgres, string schemaName, string tableName)
        {
            _postgres = postgres;
            _schemaName = schemaName;
            _tableName = tableName;
        }

        private string QualifiedTableName => @$"{_schemaName}.""{_tableName}""";

        internal async Task SetupTrigger(string body)
        {
            var triggerSql = CreateTriggerSql(body);
            await _postgres.Execute(triggerSql);

            _postgres.AddCleanUp(
                $"Drop {_tableName} trigger",
                async () => await _postgres.Execute(DropTriggerSql));
        }

        internal async Task<List<TRow>> FetchAll()
        {
            return await _postgres.Query<TRow>(FetchAllSql);
        }

        internal async Task ClearTable()
        {
            await _postgres.Execute(DeleteAllSql);
        }

        public async Task Insert(TRow row)
        {
            await _postgres.Execute(row.InsertSql(QualifiedTableName), row);
        }

        private string FetchAllSql => @$"
SELECT
    *
FROM
    {QualifiedTableName}
";

        private string DeleteAllSql => @$"
DELETE FROM {QualifiedTableName}
";

        private string CreateTriggerSql(string body) => @$"
CREATE OR REPLACE FUNCTION verify() RETURNS trigger
AS $$
BEGIN
    {body}

    RETURN NEW;
END;
$$ LANGUAGE plpgsql;

CREATE TRIGGER validate BEFORE INSERT ON {QualifiedTableName} FOR EACH ROW EXECUTE PROCEDURE verify();
";

        private string DropTriggerSql => @$"
DROP TRIGGER IF EXISTS
    validate
ON
    {QualifiedTableName}
";
    }
}