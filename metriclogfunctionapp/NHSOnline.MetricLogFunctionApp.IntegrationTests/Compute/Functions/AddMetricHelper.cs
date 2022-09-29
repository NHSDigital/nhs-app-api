using System;
using System.Threading.Tasks;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions
{
    public static class AddMetricHelper
    {
        public static async Task AddWebIntegrationReferralsMetric(TestEnv env, DateTimeOffset time, string referrer,
            string referrerOrigin, string sessionId)
        {
            await env.Postgres.Events.WebIntegrationReferrals.Insert(new WebIntegrationReferralsMetricRow()
            {
                Timestamp = time,
                Referrer = referrer,
                ReferrerOrigin = referrerOrigin,
                SessionId = sessionId,
                AuditId = Guid.NewGuid().ToString()
            });
        }

        public static async Task AddDeviceMetric(TestEnv env, DateTimeOffset timestamp, string sessionId,
            string appVersion, string deviceManufacturer, string deviceModel, string deviceOS,
            string deviceOSVersion, string userAgent)
        {
            await env.Postgres.Events.Device.Insert(new DeviceRow
            {
                Timestamp = timestamp,
                SessionId = sessionId,
                AppVersion = appVersion,
                DeviceManufacturer = deviceManufacturer,
                DeviceModel = deviceModel,
                UserAgent = userAgent,
                DeviceOS = deviceOS,
                DeviceOSVersion = deviceOSVersion
            });
        }

        public static async Task AddMedicalRecordViewMetric(TestEnv env, DateTimeOffset timestamp, string sessionId,
            bool hasDetailedRecordAccess, bool hasSummaryRecordAccess, string auditId)
        {
            await env.Postgres.Events.MedicalRecordViewMetric.Insert(new MedicalRecordViewMetricRow
            {
                Timestamp = timestamp,
                SessionId = sessionId,
                HasDetailedRecordAccess = hasDetailedRecordAccess,
                HasSummaryRecordAccess = hasSummaryRecordAccess,
                AuditId = auditId
            });
        }

        public static async Task AddMedicalRecordSectionMetric(TestEnv env, DateTimeOffset timestamp, string sessionId,
            string supplier, bool isActingOnBehalfOfAnother, string section, string auditId)
        {
            await env.Postgres.Events.MedicalRecordSectionViewMetric.Insert(new MedicalRecordSectionViewMetricRow()
            {
                Timestamp = timestamp,
                SessionId = sessionId,
                Supplier = supplier,
                IsActingOnBehalfOfAnother = isActingOnBehalfOfAnother,
                Section = section,
                AuditId = auditId
            });
        }

        public static async Task AddAppointmentBookMetric(TestEnv env, string odsCode, DateTimeOffset dateTime, string sessionId)
        {
            await env.Postgres.Events.AppointmentBookMetric.Insert(new AppointmentBookMetricRow
            {
                Timestamp = dateTime,
                SessionId = sessionId
            });
        }

        public static async Task AddAppointmentCancelMetric(TestEnv env, string odsCode, DateTimeOffset dateTime, string sessionId)
        {
            await env.Postgres.Events.AppointmentCancelMetric.Insert(new AppointmentCancelMetricRow
            {
                Timestamp = dateTime,
                SessionId = sessionId
            });
        }

        public static async Task AddNomPharmCreateMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
        {
            await env.Postgres.Events.NominatedPharmacyCreateMetric.Insert(new NominatedPharmacyCreateMetricRow
            {
                Timestamp = timestamp,
                SessionId = sessionId
            });
        }

        public static async Task AddNomPharmUpdateMetric(TestEnv env, DateTimeOffset timestamp, string sessionId)
        {
            await env.Postgres.Events.NominatedPharmacyUpdateMetric.Insert(new NominatedPharmacyUpdateMetricRow
            {
                Timestamp = timestamp,
                SessionId = sessionId
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

        public static async Task AddOrganDonationCreateMetric(TestEnv env, DateTimeOffset timestamp, string sessionId, string auditId)
        {
            await env.Postgres.Events.OrganDonationRegistrationCreateMetric.Insert(
                new OrganDonationRegistrationCreateMetricRow
                {
                    Timestamp = timestamp,
                    SessionId = sessionId,
                    AuditId = auditId
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

        public static async Task AddConsentMetric(TestEnv env, string loginId, string proofLevel, DateTimeOffset time,
            string sessionId, string odsCode = "AB123")
        {
            await env.Postgres.Events.ConsentMetric.Insert(new ConsentMetricRow
            {
                LoginId = loginId,
                OdsCode = odsCode,
                Timestamp = time,
                ProofLevel = proofLevel,
                SessionId = sessionId
            });
        }

        public static async Task AddLoginMetric(TestEnv env, string loginId, string proofLevel, DateTimeOffset time,
            string sessionId = "SessionId", string odsCode = "AB123", string loginEventId="LoginEventId")
        {
            await env.Postgres.Events.LoginMetric.Insert(new LoginMetricRow
            {
                LoginId = loginId,
                OdsCode = odsCode,
                Timestamp = time,
                ProofLevel = proofLevel,
                LoginEventId = loginEventId,
                Referrer = "Referrer",
                SessionId = sessionId,
                AuditId = Guid.NewGuid().ToString()
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
            string providerId, string providerName, string jumpOffId, string auditId)
        {
            await env.Postgres.Events.SilverIntegrationJumpOffMetric.Insert(new SilverIntegrationJumpOffMetricRow
            {
                Timestamp = dateTime,
                SessionId = sessionId,
                ProviderId = providerId,
                ProviderName = providerName,
                JumpOffId = jumpOffId,
                AuditId = auditId
            });
        }

        public static async Task AddSecondaryCareSummaryMetric(TestEnv env, DateTimeOffset dateTime, string sessionId,
            int totalReferrals, int totalUpcomingAppointments, string auditId)
        {
            await env.Postgres.Events.SecondaryCareSummaryMetric.Insert(new SecondaryCareSummaryMetricRow()
            {
                Timestamp = dateTime,
                SessionId = sessionId,
                TotalReferrals = totalReferrals,
                TotalUpcomingAppointments = totalUpcomingAppointments,
                AuditId = auditId
            });
        }
    }
}
