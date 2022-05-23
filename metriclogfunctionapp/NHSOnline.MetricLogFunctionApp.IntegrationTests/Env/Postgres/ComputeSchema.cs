using System.Runtime.CompilerServices;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class ComputeSchema
    {
        private const string SchemaName = "compute";

        private readonly PostgresWrapper _postgres;

        internal ComputeSchema(PostgresWrapper postgres) => _postgres = postgres;

        internal PostgresTable<FirstLoginsRow> FirstLogins =>
            Table<FirstLoginsRow>();

        internal PostgresTable<ReferrerLoginRow> ReferrerLogin =>
            Table<ReferrerLoginRow>();

        private PostgresTable<TRow> Table<TRow>([CallerMemberName] string viewName = "") where TRow : ITableRow
            => _postgres.Table<TRow>(SchemaName, viewName);
    }
}