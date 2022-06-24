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
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            var sessionid1 = "session1";
            var sessionid2 = "session2";

            var auditId1 = "auditId1";
            var auditId2 = "auditId2";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid2);

            await AddMetricHelper.AddOrganDonationCreateMetric(env, endDate.AddHours(-1), sessionid1, auditId1);
            await AddMetricHelper.AddOrganDonationCreateMetric(env, endDate.AddHours(-2), sessionid1, auditId2);

            await AddMetricHelper.AddOrganDonationWithdrawMetric(env, endDate.AddHours(-1), sessionid2);
            await AddMetricHelper.AddOrganDonationWithdrawMetric(env, endDate.AddHours(-2), sessionid2);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.ODRegistrations.Should().Be(2);
                row.ODWithdrawals.Should().Be(2);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_PrescriptionsEventsAreComputed(
            TestEnv env)
        {
            // Stage
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            var sessionid = "session";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid);

            await AddMetricHelper.AddPrescriptionOrderMetric(env, endDate.AddHours(-1), sessionid);
            await AddMetricHelper.AddPrescriptionOrderMetric(env, endDate.AddHours(-1), sessionid);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.Prescriptions.Should().Be(2);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_AppointmentsEventsAreComputed(
            TestEnv env)
        {
            // Stage
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            var sessionid1 = "session1";
            var sessionid2 = "session2";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid2);

            // Iv ran the SP straight onto the data this generates and it also says 4 and 4.
            await AddMetricHelper.AddAppointmentBookMetric(env, "AB123", endDate.AddHours(-1), sessionid1);
            await AddMetricHelper.AddAppointmentBookMetric(env, "AB123", endDate.AddHours(-1), sessionid1);

            await AddMetricHelper.AddAppointmentCancelMetric(env, "AB123", endDate.AddHours(-1), sessionid2);
            await AddMetricHelper.AddAppointmentCancelMetric(env, "AB123", endDate.AddHours(-1), sessionid2);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.AppointmentsBooked.Should().Be(2);
                row.AppointmentsCancelled.Should().Be(2);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_RanWithWebReferrerRows_Expect_OnlyRowsInDateRangeAreProcessed(
            TestEnv env)
        {
            // Stage
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", "session1");
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-2), "ref", "session2");
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-3), "ref", "session3");

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddDays(-2), "ref", "session4");

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.Logins.Should().Be(3);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_RanWithWebReferrerRows_Expect_LoginIDsAreCountedUniquely(TestEnv env)
        {
            // Stage
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            var loginId = "LoginId1";

            var sessionid1 = "session1";
            var sessionid2 = "session2";

            await AddMetricHelper.AddLoginMetric(env, loginId,"P9",endDate.AddHours(-1), sessionid1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid1);

            await AddMetricHelper.AddLoginMetric(env, loginId,"P9",endDate.AddHours(-1), sessionid2);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid2);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.Logins.Should().Be(2);
                row.Users.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_RanWithEmptyDeviceOS_Expect_DeviceOSBecomesUnknown(TestEnv env)
        {
            // Stage
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            var sessionid = "session";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid);
            await AddMetricHelper.AddDeviceMetric(env, endDate.AddHours(-1), sessionid, "appVersion",
                "test", "testPlus", null, "0", "userAgent");

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.DeviceOS.Should().Be("Unknown");
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_DcrOrScrIsSet_Expect_tCountedAs1(
            TestEnv env)
        {
            // Stage
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            var sessionid1 = "session1";
            var sessionid2 = "session2";

            var auditId1 = "auditId1";
            var auditId2 = "auditId2";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid1);
            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid2);

            await AddMetricHelper.AddMedicalRecordViewMetric(env, endDate.AddHours(-1), sessionid1, false, true, auditId1);
            await AddMetricHelper.AddMedicalRecordViewMetric(env, endDate.AddHours(-1), sessionid2, true, true, auditId2);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.RecordViewsSCR.Should().Be(2);
                row.RecordViewsDCR.Should().Be(1);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_CalledWithNomPharmacyCreatedAndUpdated_Expect_NomPharmacyIsTheSumOfCreatesAndUpdates(
            TestEnv env)
        {
            // Stage
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            var sessionid = "session";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", sessionid);

            await AddMetricHelper.AddNomPharmCreateMetric(env, endDate.AddHours(-1), sessionid);
            await AddMetricHelper.AddNomPharmUpdateMetric(env, endDate.AddHours(-1), sessionid);

            // Act
            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.NomPharmacy.Should().Be(2);
            }
        }

        [NhsAppTest]
        public async Task DailyDeviceReferral_When_UniqueConstraintIsViolated_Expect_RowToBeInsertedReplacesExistingRow(TestEnv env)
        {
            // Stage
            var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
            var endDateString = "2022-05-27T00:00:00";
            var startDateString = "2022-05-26T00:00:00";

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", "session");

            var response = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            response.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.Users.Should().Be(0);
            }

            await AddMetricHelper.AddWebIntegrationReferralsMetric(env, endDate.AddHours(-1), "ref", "session2");
            await AddMetricHelper.AddLoginMetric(env, "loginID","P9",endDate.AddHours(-1), "session2");

            //Act
            var secondResponse = await env.HttpEndpointCallers.PostDailyDeviceReferralUsage(startDateString, endDateString);
            secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);
            await env.Queues.DailyDeviceReferralUsage.WaitUntilEmpty();

            var newRows = await env.Postgres.Compute.DailyDeviceReferralUsage.FetchAll();

            // Assert
            using (new AssertionScope())
            {
                newRows.Count.Should().Be(1);
                var row = newRows.Single(x => x.Date == DateTime.Parse(startDateString));

                row.Users.Should().Be(1);
            }

        }
    }
}
