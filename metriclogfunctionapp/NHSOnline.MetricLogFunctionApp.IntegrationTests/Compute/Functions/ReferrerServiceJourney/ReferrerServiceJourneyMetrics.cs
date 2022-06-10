using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.ReferrerServiceJourney;

internal static class ReferrerServiceJourneyMetrics
{
    public static async Task AddMedicalRecordViewMetric(TestEnv env, DateTimeOffset time,
        string sessionId, bool hasDetailedRecordAccess, bool hasSummaryRecordAccess, string auditId)
    {
        await env.Postgres.Events.MedicalRecordViewMetric.Insert(new MedicalRecordViewMetricRow
        {
            Timestamp = time,
            SessionId = sessionId,
            HasDetailedRecordAccess = hasDetailedRecordAccess,
            HasSummaryRecordAccess = hasSummaryRecordAccess,
            AuditId = auditId,
        });
    }

    public static async Task AddPrescriptionOrderMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
    {
        await env.Postgres.Events.PrescriptionOrdersMetric.Insert(new PrescriptionOrdersMetricRow
        {
            Timestamp = timestamp,
            SessionId = sessionId
        });
    }

    public static async Task AddOrganDonationCreateMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
    {
        await env.Postgres.Events.OrganDonationRegistrationCreateMetric.Insert(
            new OrganDonationRegistrationCreateMetricRow
            {
                Timestamp = timestamp,
                SessionId = sessionId
            });
    }

    public static async Task AddOrganDonationWithdrawMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
    {
        await env.Postgres.Events.OrganDonationRegistrationWithdrawMetric.Insert(
            new OrganDonationRegistrationWithdrawMetricRow
            {
                Timestamp = timestamp,
                SessionId = sessionId
            });
    }

    public static async Task AddOrganDonationUpdateMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
    {
        await env.Postgres.Events.OrganDonationRegistrationUpdateMetric.Insert(
            new OrganDonationRegistrationUpdateMetricRow
            {
                Timestamp = timestamp,
                SessionId = sessionId
            });
    }

    public static async Task AddOrganDonationGetMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
    {
        await env.Postgres.Events.OrganDonationRegistrationGetMetric.Insert(
            new OrganDonationRegistrationGetMetricRow
            {
                Timestamp = timestamp,
                SessionId = sessionId
            });
    }

    public static async Task AddNomPharmacyUpdateMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
    {
        await env.Postgres.Events.NominatedPharmacyUpdateMetric.Insert(new NominatedPharmacyUpdateMetricRow
        {
            Timestamp = timestamp,
            SessionId = sessionId
        });
    }

    public static async Task AddNomPharmacyCreateMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
    {
        await env.Postgres.Events.NominatedPharmacyCreateMetric.Insert(new NominatedPharmacyCreateMetricRow
        {
            Timestamp = timestamp,
            SessionId = sessionId
        });
    }

    public static async Task AddAppointmentBookMetric(TestEnv env, DateTimeOffset dateTime, string sessionId)
    {
        await env.Postgres.Events.AppointmentBookMetric.Insert(new AppointmentBookMetricRow
        {
            Timestamp = dateTime,
            SessionId = sessionId
        });
    }

    public static async Task AddAppointmentCancelMetric(TestEnv env, DateTimeOffset dateTime, string sessionId)
    {
        await env.Postgres.Events.AppointmentCancelMetric.Insert(new AppointmentCancelMetricRow
        {
            Timestamp = dateTime,
            SessionId = sessionId
        });
    }

    public static async Task AddSilverIntegrationJumpOffMetric(TestEnv env, DateTimeOffset dateTime, string sessionId,
        string providerId, string providerName, string jumpOffId)
    {
        await env.Postgres.Events.SilverIntegrationJumpOffMetric.Insert(new SilverIntegrationJumpOffMetricRow
        {
            Timestamp = dateTime,
            SessionId = sessionId,
            ProviderId = providerId,
            ProviderName = providerName,
            JumpOffId = jumpOffId
        });
    }

    public static async Task AddWebIntegrationReferralsMetric(TestEnv env, DateTimeOffset time, string sessionId, string referrer)
    {
        await env.Postgres.Events.WebIntegrationReferrals.Insert(new WebIntegrationReferralsMetricRow
        {
            Timestamp = time,
            Referrer = referrer,
            SessionId = sessionId,
            AuditId = Guid.NewGuid().ToString()
        });
    }
}