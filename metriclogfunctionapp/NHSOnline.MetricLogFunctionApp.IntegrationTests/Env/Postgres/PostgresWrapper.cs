using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Dapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using NHSOnline.MetricLogFunctionApp.Database.EntityFramework;
using Npgsql;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class PostgresWrapper
    {
        private readonly List<(string description, Func<Task> cleanUpAction)> _cleanUp = new List<(string description, Func<Task> cleanUpAction)>();

        static PostgresWrapper()
        {
            SqlMapper.RemoveTypeMap(typeof(DateTimeOffset));
            SqlMapper.AddTypeHandler(new DateTimeOffsetTypeHandler());
        }

        internal PostgresWrapper(TestLogs logs)
        {
            Logs = logs;
        }

        private TestLogs Logs { get; }

        internal EventsSchema Events => new EventsSchema(this);
        internal ComputeSchema Compute => new ComputeSchema(this);

        internal AuditSchema Audit => new AuditSchema(this);
        internal TransactionContext TransactionContext { get; private set; }

        internal async Task Initialise()
        {
            var tables = await Query<string>(SelectTablesSql);
            foreach (var tableName in tables)
            {
                var resetSql = ResetTableSql(tableName);
                await Execute(resetSql);
            }
            InitTransactionContext();
        }

        internal async Task Execute(string query, object param = null)
        {
            Logs.Info("Executing {0}", query);

            await using var connection = new NpgsqlConnection("Host=localhost;Database=analyticsdb;Port=5432;User Id=pgsadmin;Password=password");

            await connection.OpenAsync();
            var result = await connection.ExecuteAsync(query, param);

            Logs.Info("Execute returned {0}", result);
        }

        internal async Task<List<T>> Query<T>(string query)
        {
            Logs.Info("Executing {0}", query);

            await using var connection = new NpgsqlConnection("Host=localhost;Database=analyticsdb;Port=5432;User Id=pgsadmin;Password=password");

            await connection.OpenAsync();
            var results = await connection.QueryAsync<T>(query);
            var resultsList = results.ToList();

            Logs.Info("Execute returning {0} rows", resultsList.Count);

            return resultsList;
        }

        internal void AddCleanUp(string description, Func<Task> cleanUpAction) => _cleanUp.Add((description, cleanUpAction));

        public async Task CleanUp(TestResultContext testResultContext)
        {
            foreach (var (description, cleanUpAction) in _cleanUp)
            {
                await testResultContext.TryCleanUp(description, cleanUpAction);
            }
        }

        public void InitTransactionContext()
        {
            var contextOptions = new DbContextOptionsBuilder<TransactionContext>()
                .UseNpgsql("Host=localhost;Database=analyticsdb;Port=5432;User Id=pgsadmin;Password=password")
                .Options;
            TransactionContext = new TransactionContext(
                contextOptions,
                new List<IDbConnectionInterceptor>());
        }

        private static string SelectTablesSql => @"
SELECT
    CONCAT('""', schemaname, '"".""', tablename, '""')
FROM
    pg_tables
WHERE
    schemaname IN ('events', 'reports', 'compute', 'audit')
";

        private static string ResetTableSql(string qualifiedTableName) => $@"
DROP TRIGGER IF EXISTS
    validate
ON
    {qualifiedTableName};

TRUNCATE TABLE {qualifiedTableName}
";

        internal PostgresView<TRow> View<TRow>(string schemaName, string viewName)
            => new PostgresView<TRow>(this, schemaName, viewName);

        public PostgresTable<TRow> Table<TRow>(string schemaName, string viewName) where TRow : ITableRow
            => new PostgresTable<TRow>(this, schemaName, viewName);
    }
}