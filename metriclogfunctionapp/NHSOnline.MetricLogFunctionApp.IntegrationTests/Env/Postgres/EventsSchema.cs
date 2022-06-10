using System.Runtime.CompilerServices;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres
{
    internal sealed class EventsSchema
    {
        private const string SchemaName = "events";

        private readonly PostgresWrapper _postgres;

        internal EventsSchema(PostgresWrapper postgres) => _postgres = postgres;

        internal PostgresTable<AppointmentBookMetricRow> AppointmentBookMetric =>
            Table<AppointmentBookMetricRow>();

        internal PostgresTable<AppointmentCancelMetricRow> AppointmentCancelMetric =>
            Table<AppointmentCancelMetricRow>();

        internal PostgresTable<ConsentMetricRow> ConsentMetric => Table<ConsentMetricRow>();

        internal PostgresTable<DeviceRow> Device => Table<DeviceRow>();

        internal PostgresTable<LoginMetricRow> LoginMetric => Table<LoginMetricRow>();

        internal PostgresTable<WebIntegrationReferralsMetricRow> WebIntegrationReferrals => Table<WebIntegrationReferralsMetricRow>();

        internal PostgresTable<SecondaryCareSummaryMetricRow> SecondaryCareSummaryMetric => Table<SecondaryCareSummaryMetricRow>();

        internal PostgresTable<MedicalRecordViewMetricRow> MedicalRecordViewMetric =>
            Table<MedicalRecordViewMetricRow>();

        internal PostgresTable<NominatedPharmacyCreateMetricRow> NominatedPharmacyCreateMetric =>
            Table<NominatedPharmacyCreateMetricRow>();

        internal PostgresTable<NominatedPharmacyUpdateMetricRow> NominatedPharmacyUpdateMetric =>
            Table<NominatedPharmacyUpdateMetricRow>();

        internal PostgresTable<NotificationToggleMetricRow> NotificationToggleMetric => Table<NotificationToggleMetricRow>();

        internal PostgresTable<OrganDonationRegistrationCreateMetricRow> OrganDonationRegistrationCreateMetric =>
            Table<OrganDonationRegistrationCreateMetricRow>();

        internal PostgresTable<OrganDonationRegistrationWithdrawMetricRow> OrganDonationRegistrationWithdrawMetric =>
            Table<OrganDonationRegistrationWithdrawMetricRow>();

        internal PostgresTable<OrganDonationRegistrationUpdateMetricRow> OrganDonationRegistrationUpdateMetric =>
            Table<OrganDonationRegistrationUpdateMetricRow>();

        internal PostgresTable<OrganDonationRegistrationGetMetricRow> OrganDonationRegistrationGetMetric =>
            Table<OrganDonationRegistrationGetMetricRow>();

        internal PostgresTable<PrescriptionOrdersMetricRow> PrescriptionOrdersMetric =>
            Table<PrescriptionOrdersMetricRow>();

        internal PostgresTable<InitialPromptMetricRow> InitialPromptMetric => Table<InitialPromptMetricRow>();

        internal PostgresTable<SilverIntegrationJumpOffMetricRow> SilverIntegrationJumpOffMetric =>
            Table<SilverIntegrationJumpOffMetricRow>();

        private PostgresTable<TRow> Table<TRow>([CallerMemberName] string viewName = "") where TRow : ITableRow
            => _postgres.Table<TRow>(SchemaName, viewName);

        private PostgresView<TRow> View<TRow>([CallerMemberName] string viewName = "")
            => _postgres.View<TRow>(SchemaName, viewName);
    }
}