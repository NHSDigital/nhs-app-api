using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.DailyDeviceReferralUsage
{
    [TestClass]
    public class DailyDeviceReferralTests
    {
        [NhsAppTest]
        public async Task DailyDeviceReferral_OrganDonationEventsAreComputed(
            TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            var sessionid1 = "session1";
            var sessionid2 = "session2";

            var auditId1 = "auditId1";
            var auditId2 = "auditId2";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid2);

            await AddMetricHelper.AddOrganDonationCreateMetric(env, endDateTime.AddHours(-1), sessionid1, auditId1);
            await AddMetricHelper.AddOrganDonationCreateMetric(env, endDateTime.AddHours(-2), sessionid1, auditId2);

            await AddMetricHelper.AddOrganDonationWithdrawMetric(env, endDateTime.AddHours(-1), sessionid2);
            await AddMetricHelper.AddOrganDonationWithdrawMetric(env, endDateTime.AddHours(-2), sessionid2);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.ODRegistrations.Should().Be(2);
                row.ODWithdrawals.Should().Be(2);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_PrescriptionsEventsAreComputed(
            TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            var sessionid = "session";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid);

            await AddMetricHelper.AddPrescriptionOrderMetric(env, endDateTime.AddHours(-1), sessionid);
            await AddMetricHelper.AddPrescriptionOrderMetric(env, endDateTime.AddHours(-1), sessionid);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.Prescriptions.Should().Be(2);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_AppointmentsEventsAreComputed(
            TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            var sessionid1 = "session1";
            var sessionid2 = "session2";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid2);

            // Iv ran the SP straight onto the data this generates and it also says 4 and 4.
            await AddMetricHelper.AddAppointmentBookMetric(env, "AB123", endDateTime.AddHours(-1), sessionid1, false);
            await AddMetricHelper.AddAppointmentBookMetric(env, "AB123", endDateTime.AddHours(-1), sessionid1, true);

            await AddMetricHelper.AddAppointmentCancelMetric(env, "AB123", endDateTime.AddHours(-1), sessionid2);
            await AddMetricHelper.AddAppointmentCancelMetric(env, "AB123", endDateTime.AddHours(-1), sessionid2);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.AppointmentsBooked.Should().Be(2);
                row.AppointmentsCancelled.Should().Be(2);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_RanWithWebReferrerRows_Expect_OnlyRowsInDateRangeAreProcessed(
            TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", "session1");
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-2), "ref", "refOrigin", "session2");
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-3), "ref", "refOrigin", "session3");

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddDays(-2), "ref", "refOrigin", "session4" );

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.Logins.Should().Be(3);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_RanWithWebReferrerRows_Expect_LoginIDsAreCountedUniquely(TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            var loginId = "LoginId1";

            var sessionid1 = "session1";
            var sessionid2 = "session2";

            await AddMetricHelper.AddLoginMetric(env, loginId,"P9",endDateTime.AddHours(-1), sessionid1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid1);

            await AddMetricHelper.AddLoginMetric(env, loginId,"P9",endDateTime.AddHours(-1), sessionid2);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid2);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.Logins.Should().Be(2);
                row.Users.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_RanWithEmptyDeviceOS_Expect_DeviceOSBecomesUnknown(TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            var sessionid = "session";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid);
            await AddMetricHelper.AddDeviceMetric(env, endDateTime.AddHours(-1), sessionid, "appVersion",
                "test", "testPlus", null, "0", "userAgent");

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.DeviceOS.Should().Be("Unknown");
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_DcrOrScrIsSet_Expect_tCountedAs1(
            TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            var sessionid1 = "session1";
            var sessionid2 = "session2";

            var auditId1 = "auditId1";
            var auditId2 = "auditId2";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid2);

            await AddMetricHelper.AddMedicalRecordViewMetric(env, endDateTime.AddHours(-1), sessionid1, false, true, auditId1);
            await AddMetricHelper.AddMedicalRecordViewMetric(env, endDateTime.AddHours(-1), sessionid2, true, true, auditId2);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.RecordViewsSCR.Should().Be(2);
                row.RecordViewsDCR.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_CalledWithNomPharmacyCreatedAndUpdated_Expect_NomPharmacyIsTheSumOfCreatesAndUpdates(
            TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            var sessionid = "session";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", sessionid);

            await AddMetricHelper.AddNomPharmCreateMetric(env, endDateTime.AddHours(-1), sessionid);
            await AddMetricHelper.AddNomPharmUpdateMetric(env, endDateTime.AddHours(-1), sessionid);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.NomPharmacy.Should().Be(2);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_UniqueConstraintIsViolated_Expect_RowToBeInsertedReplacesExistingRow(TestEnv env)
        {
            // Stage
            var endDateTime = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateTimeString = "2022-05-27T00:00:00";
            var startDateTimeString = "2022-05-26T00:00:00";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", "session");

            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.Users.Should().Be(0);
            }

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDateTime.AddHours(-1), "ref", "refOrigin", "session2");
            await AddMetricHelper.AddLoginMetric(env, "loginID","P9",endDateTime.AddHours(-1), "session2");

            //Act
            var secondResponse = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateTimeString, endDateTimeString);
            secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var newRows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                newRows.Count.Should().Be(1);
                var row = newRows.Single(x => x.Date == DateTime.Parse(startDateTimeString));

                row.Users.Should().Be(1);
            }
        }
    }
}
