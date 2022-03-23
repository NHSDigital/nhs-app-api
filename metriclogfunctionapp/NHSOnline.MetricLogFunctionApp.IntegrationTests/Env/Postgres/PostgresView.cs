using System.Collections.Generic;
using System.Threading.Tasks;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class PostgresView<TRow>
    {
        private readonly PostgresWrapper _postgres;
        private readonly string _schemaName;
        private readonly string _viewName;

        public PostgresView(PostgresWrapper postgres, string schemaName, string viewName)
        {
            _postgres = postgres;
            _schemaName = schemaName;
            _viewName = viewName;
        }

        private string QualifiedViewName => @$"{_schemaName}.""{_viewName}""";

        internal async Task<List<TRow>> FetchAll()
        {
            return await _postgres.Query<TRow>(FetchAllSql);
        }

        private string FetchAllSql => @$"
SELECT
    *
FROM
    {QualifiedViewName}
";
    }
}