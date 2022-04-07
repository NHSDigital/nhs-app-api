using System.Runtime.CompilerServices;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Audit;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class AuditSchema
    {
        private const string SchemaName = "audit";

        private readonly PostgresWrapper _postgres;

        internal AuditSchema(PostgresWrapper postgres) => _postgres = postgres;

        internal PostgresTable<ProcessRow> Process =>
                    Table<ProcessRow>();

        internal PostgresTable<ProcessDurationRow> ProcessDuration =>
            Table<ProcessDurationRow>();

        private PostgresTable<TRow> Table<TRow>([CallerMemberName] string viewName = "") where TRow : ITableRow
            => _postgres.Table<TRow>(SchemaName, viewName);
    }
}