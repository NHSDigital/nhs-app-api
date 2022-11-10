using System.Runtime.CompilerServices;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class ComputeSchema
    {
        private const string SchemaName = "compute";

        private readonly PostgresWrapper _postgres;

        internal ComputeSchema(PostgresWrapper postgres) => _postgres = postgres;

        internal PostgresTable<DailyDeviceReferralUsageRow> DailyDeviceReferralUsage =>
            Table<DailyDeviceReferralUsageRow>();

        internal PostgresTable<DeviceInfoRow> DeviceInfo =>
            Table<DeviceInfoRow>();

        internal PostgresTable<FirstLoginsRow> FirstLogins =>
            Table<FirstLoginsRow>();

        internal PostgresTable<ReferrerLoginRow> ReferrerLogin =>
            Table<ReferrerLoginRow>();

        internal PostgresTable<ReferrerServiceJourneyRow> ReferrerServiceJourney =>
            Table<ReferrerServiceJourneyRow>();

        internal PostgresTable<GPRecordViewsRows> GPRecordViews =>
            Table<GPRecordViewsRows>();

        internal PostgresTable<TestOdsCodesRow> TestOdsCodes =>
            Table<TestOdsCodesRow>();

        internal PostgresTable<WayfinderRow> Wayfinder =>
            Table<WayfinderRow>();

        private PostgresTable<TRow> Table<TRow>([CallerMemberName] string viewName = "") where TRow : ITableRow
            => _postgres.Table<TRow>(SchemaName, viewName);
    }
}
