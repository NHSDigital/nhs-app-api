using System.Runtime.CompilerServices;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class EventsSchema
    {
        private const string SchemaName = "events";

        private readonly PostgresWrapper _postgres;

        internal EventsSchema(PostgresWrapper postgres) => _postgres = postgres;

        internal PostgresTable<ConsentMetricRow> ConsentMetric => Table<ConsentMetricRow>();

        internal PostgresTable<LoginMetricRow> LoginMetric => Table<LoginMetricRow>();

        internal PostgresTable<WebIntegrationReferralsMetricRow> WebIntegrationReferrals => Table<WebIntegrationReferralsMetricRow>();


        private PostgresTable<TRow> Table<TRow>([CallerMemberName] string viewName = "") where TRow : ITableRow
            => _postgres.Table<TRow>(SchemaName, viewName);

        private PostgresView<TRow> View<TRow>([CallerMemberName] string viewName = "")
            => _postgres.View<TRow>(SchemaName, viewName);
    }
}