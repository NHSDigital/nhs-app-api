using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Etl.Functions.AuditLog
{
    [TestClass]
    public class AuditLogConsumerTests
    {
        [NhsAppTest]
        public async Task AuditLog_AuditLogEventHubMessages_AreLoaded(TestEnv env)
        {
            var events = BuildEvents();
            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginMetricRows = await env.Postgres.Events.LoginMetric.FetchAll();
            loginMetricRows.Should().HaveCount(2);

            var webIntegrationReferralsMetricRows = await env.Postgres.Events.WebIntegrationReferrals.FetchAll();
            webIntegrationReferralsMetricRows.Should().HaveCount(2);

            var consentMetricRows = await env.Postgres.Events.ConsentMetric.FetchAll();
            consentMetricRows.Should().HaveCount(3);

            var secondaryCareSummaryMetricRows = await env.Postgres.Events.SecondaryCareSummaryMetric.FetchAll();
            secondaryCareSummaryMetricRows.Should().HaveCount(1);

            var medicalRecordViewMetricRows = await env.Postgres.Events.MedicalRecordViewMetric.FetchAll();
            medicalRecordViewMetricRows.Should().HaveCount(1);

            var notificationToggleMetricRows = await env.Postgres.Events.NotificationToggleMetric.FetchAll();
            notificationToggleMetricRows.Should().HaveCount(1);

            var initialPromptMetricRows = await env.Postgres.Events.InitialPromptMetric.FetchAll();
            initialPromptMetricRows.Should().HaveCount(1);

            var appointmentCancelMetricRows = await env.Postgres.Events.AppointmentCancelMetric.FetchAll();
            appointmentCancelMetricRows.Should().HaveCount(1);

            var appointmentBookMetricRows = await env.Postgres.Events.AppointmentBookMetric.FetchAll();
            appointmentBookMetricRows.Should().HaveCount(1);

            var organDonationRegistrationGetMetricRows = await env.Postgres.Events.OrganDonationRegistrationGetMetric.FetchAll();
            organDonationRegistrationGetMetricRows.Should().HaveCount(1);

            var organDonationRegistrationCreateMetricRows = await env.Postgres.Events.OrganDonationRegistrationCreateMetric.FetchAll();
            organDonationRegistrationCreateMetricRows.Should().HaveCount(1);

            var repeatPrescriptionMetricRows = await env.Postgres.Events.PrescriptionOrdersMetric.FetchAll();
            repeatPrescriptionMetricRows.Should().HaveCount(1);

            var organDonationRegistrationWithdrawMetricRows = await env.Postgres.Events.OrganDonationRegistrationWithdrawMetric.FetchAll();
            organDonationRegistrationWithdrawMetricRows.Should().HaveCount(1);

            var organDonationRegistrationUpdateMetricRows = await env.Postgres.Events.OrganDonationRegistrationUpdateMetric.FetchAll();
            organDonationRegistrationUpdateMetricRows.Should().HaveCount(1);

            var biometricsToggleMetricRows = await env.Postgres.Events.BiometricsToggleMetric.FetchAll();
            biometricsToggleMetricRows.Should().HaveCount(1);

            var medicalRecordSectionViewMetricRows = await env.Postgres.Events.MedicalRecordSectionViewMetric.FetchAll();
            medicalRecordSectionViewMetricRows.Should().HaveCount(1);

            using (new AssertionScope())
            {
                var loginMetricRow1 = loginMetricRows.First(r => r.SessionId == "TestSession1");
                loginMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.001Z"));
                loginMetricRow1.OdsCode.Should().Be("ods1");
                loginMetricRow1.LoginId.Should().Be("NhsLoginSubject-Test");
                loginMetricRow1.ProofLevel.Should().Be("P5");
                loginMetricRow1.Referrer.Should().Be("ref1");
                loginMetricRow1.AuditId.Should().Be("AuditId1");

                var loginMetricRow2 = loginMetricRows.First(r => r.SessionId == "TestSession2");
                loginMetricRow2.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.002Z"));
                loginMetricRow2.OdsCode.Should().Be("ods2");
                loginMetricRow2.LoginId.Should().Be("NhsLoginSubject-Test");
                loginMetricRow2.ProofLevel.Should().Be("P9");
                loginMetricRow2.Referrer.Should().Be("ref2");
                loginMetricRow2.AuditId.Should().Be("AuditId2");

                var webIntegrationReferralsMetricRow1 = webIntegrationReferralsMetricRows.First(r => r.SessionId == "TestSession1");
                webIntegrationReferralsMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.001Z"));
                webIntegrationReferralsMetricRow1.Referrer.Should().Be("Int-ref1");
                webIntegrationReferralsMetricRow1.AuditId.Should().Be("AuditId1");
                webIntegrationReferralsMetricRow1.ReferrerOrigin.Should().Be("ref-Org1");

                var webIntegrationReferralsMetricRow2 = webIntegrationReferralsMetricRows.First(r => r.SessionId == "TestSession2");
                webIntegrationReferralsMetricRow2.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.002Z"));
                webIntegrationReferralsMetricRow2.Referrer.Should().Be("Int-ref2");
                webIntegrationReferralsMetricRow2.AuditId.Should().Be("AuditId2");
                webIntegrationReferralsMetricRow2.ReferrerOrigin.Should().Be(null);

                var consentMetricRow1 = consentMetricRows.First(r => r.SessionId == "TestSession3");
                consentMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.003Z"));
                consentMetricRow1.AuditId.Should().Be("AuditId3");

                var consentMetricRow2 = consentMetricRows.First(r => r.SessionId == "TestSession4");
                consentMetricRow2.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.004Z"));
                consentMetricRow2.AuditId.Should().Be("AuditId4");

                var consentMetricRow3 = consentMetricRows.First(r => r.SessionId == "TestSession5");
                consentMetricRow3.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.005Z"));
                consentMetricRow3.AuditId.Should().Be("AuditId5");

                var secondaryCareSummaryMetricRow1 = secondaryCareSummaryMetricRows.First(r => r.SessionId == "TestSession6");
                secondaryCareSummaryMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.006Z"));
                secondaryCareSummaryMetricRow1.TotalReferrals.Should().Be(123);
                secondaryCareSummaryMetricRow1.TotalUpcomingAppointments.Should().Be(456);
                secondaryCareSummaryMetricRow1.AuditId.Should().Be("AuditId6");

                var medicalRecordViewMetricRow1 = medicalRecordViewMetricRows.First(r => r.SessionId == "TestSession7");
                medicalRecordViewMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.007Z"));
                medicalRecordViewMetricRow1.HasSummaryRecordAccess.Should().Be(true);
                medicalRecordViewMetricRow1.HasDetailedRecordAccess.Should().Be(false);
                medicalRecordViewMetricRow1.AuditId.Should().Be("AuditId7");

                var notificationToggleMetricRow = notificationToggleMetricRows.First(r => r.LoginId == "NhsLoginSubject-Test");
                notificationToggleMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.008Z"));
                notificationToggleMetricRow.NotificationToggle.Should().Be("On");
                notificationToggleMetricRow.AuditId.Should().Be("AuditId8");

                var initialPromptMetricRow = initialPromptMetricRows.First(r => r.LoginId == "NhsLoginSubject-Test");
                initialPromptMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.007Z"));
                initialPromptMetricRow.OptedIn.Should().Be("On");
                initialPromptMetricRow.AuditId.Should().Be("AuditId9");

                var appointmentCancelMetricRow1 = appointmentCancelMetricRows.First(r => r.SessionId == "TestSession10");
                appointmentCancelMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.010Z"));
                appointmentCancelMetricRow1.AuditId.Should().Be("AuditId10");

                var appointmentBookMetricRow1 = appointmentBookMetricRows.First(r => r.SessionId == "TestSession11");
                appointmentBookMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.011Z"));
                appointmentBookMetricRow1.AuditId.Should().Be("AuditId11");
                appointmentBookMetricRow1.IsActingOnBehalfOfAnother.Should().Be(false);

                var organDonationRegistrationGetMetricRow1 = organDonationRegistrationGetMetricRows.First(r => r.SessionId == "TestSession12");
                organDonationRegistrationGetMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.012Z"));
                organDonationRegistrationGetMetricRow1.AuditId.Should().Be("AuditId12");

                var organDonationRegistrationCreateMetricRow1 = organDonationRegistrationCreateMetricRows.First(r => r.SessionId == "TestSession13");
                organDonationRegistrationCreateMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.013Z"));
                organDonationRegistrationCreateMetricRow1.AuditId.Should().Be("AuditId13");

                var repeatPrescriptionMetricRow1 = repeatPrescriptionMetricRows.First(r => r.SessionId == "TestSession14");
                repeatPrescriptionMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.014Z"));
                repeatPrescriptionMetricRow1.AuditId.Should().Be("AuditId14");

                var organDonationRegistrationUpdateMetricRow1 = organDonationRegistrationUpdateMetricRows.First(r => r.SessionId == "TestSession16");
                organDonationRegistrationUpdateMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.016Z"));
                organDonationRegistrationUpdateMetricRow1.AuditId.Should().Be("AuditId16");

                var biometricsToggleMetricRow = biometricsToggleMetricRows.First(r => r.SessionId == "TestSession17");
                biometricsToggleMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.008Z"));
                biometricsToggleMetricRow.BiometricsToggle.Should().Be("On");
                biometricsToggleMetricRow.AuditId.Should().Be("AuditId17");

                var medicalRecordSectionViewMetricRow1 = medicalRecordSectionViewMetricRows.First(r => r.SessionId == "TestSession18");
                medicalRecordSectionViewMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.018Z"));
                medicalRecordSectionViewMetricRow1.Supplier.Should().Be("Supplier-Test");
                medicalRecordSectionViewMetricRow1.IsActingOnBehalfOfAnother.Should().Be(false);
                medicalRecordSectionViewMetricRow1.Section.Should().Be("TEST RESULTS");
                medicalRecordSectionViewMetricRow1.AuditId.Should().Be("AuditId18");
            }
        }

        [NhsAppTest]
        public async Task AuditLog_AuditLogEventHubMessages_WhenEventIsRepeated_DuplicatesNotLoaded(TestEnv env)
        {
            var events = BuildEvents();
            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var duplicateEventsResponse = await env.HttpEndpointCallers.PostAuditLogConsumer(events);

            duplicateEventsResponse.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginMetricRows = await env.Postgres.Events.LoginMetric.FetchAll();
            loginMetricRows.Should().HaveCount(2);

            var webIntegrationReferralsMetricRows = await env.Postgres.Events.WebIntegrationReferrals.FetchAll();
            webIntegrationReferralsMetricRows.Should().HaveCount(2);

            var consentMetricRows = await env.Postgres.Events.ConsentMetric.FetchAll();
            consentMetricRows.Should().HaveCount(3);

            var secondaryCareSummaryMetricRows = await env.Postgres.Events.SecondaryCareSummaryMetric.FetchAll();
            secondaryCareSummaryMetricRows.Should().HaveCount(1);

            var medicalRecordViewMetricRows = await env.Postgres.Events.MedicalRecordViewMetric.FetchAll();
            medicalRecordViewMetricRows.Should().HaveCount(1);

            var notificationToggleMetricRows = await env.Postgres.Events.NotificationToggleMetric.FetchAll();
            notificationToggleMetricRows.Should().HaveCount(1);

            var initialPromptMetricRows = await env.Postgres.Events.InitialPromptMetric.FetchAll();
            initialPromptMetricRows.Should().HaveCount(1);

            var organDonationRegistrationGetMetricRows = await env.Postgres.Events.OrganDonationRegistrationGetMetric.FetchAll();
            organDonationRegistrationGetMetricRows.Should().HaveCount(1);

            var appointmentCancelMetricRows = await env.Postgres.Events.AppointmentCancelMetric.FetchAll();
            appointmentCancelMetricRows.Should().HaveCount(1);

            var appointmentBookMetricRows = await env.Postgres.Events.AppointmentBookMetric.FetchAll();
            appointmentBookMetricRows.Should().HaveCount(1);

            var organDonationRegistrationCreateMetricRows = await env.Postgres.Events.OrganDonationRegistrationCreateMetric.FetchAll();
            organDonationRegistrationCreateMetricRows.Should().HaveCount(1);

            var repeatPrescriptionMetricRows = await env.Postgres.Events.PrescriptionOrdersMetric.FetchAll();
            repeatPrescriptionMetricRows.Should().HaveCount(1);

            var organDonationRegistrationWithdrawMetricRows = await env.Postgres.Events.OrganDonationRegistrationWithdrawMetric.FetchAll();
            organDonationRegistrationWithdrawMetricRows.Should().HaveCount(1);

            var organDonationRegistrationUpdateMetricRows = await env.Postgres.Events.OrganDonationRegistrationUpdateMetric.FetchAll();
            organDonationRegistrationUpdateMetricRows.Should().HaveCount(1);

            var biometricsToggleMetricRows = await env.Postgres.Events.BiometricsToggleMetric.FetchAll();
            biometricsToggleMetricRows.Should().HaveCount(1);

            var medicalRecordSectionViewMetricRows = await env.Postgres.Events.MedicalRecordSectionViewMetric.FetchAll();
            medicalRecordSectionViewMetricRows.Should().HaveCount(1);

            using (new AssertionScope())
            {
                var loginMetricRow1 = loginMetricRows.First(r => r.SessionId == "TestSession1");
                loginMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.001Z"));
                loginMetricRow1.OdsCode.Should().Be("ods1");
                loginMetricRow1.LoginId.Should().Be("NhsLoginSubject-Test");
                loginMetricRow1.ProofLevel.Should().Be("P5");
                loginMetricRow1.Referrer.Should().Be("ref1");
                loginMetricRow1.AuditId.Should().Be("AuditId1");

                var loginMetricRow2 = loginMetricRows.First(r => r.SessionId == "TestSession2");
                loginMetricRow2.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.002Z"));
                loginMetricRow2.OdsCode.Should().Be("ods2");
                loginMetricRow2.LoginId.Should().Be("NhsLoginSubject-Test");
                loginMetricRow2.ProofLevel.Should().Be("P9");
                loginMetricRow2.Referrer.Should().Be("ref2");
                loginMetricRow2.AuditId.Should().Be("AuditId2");

                var webIntegrationReferralsMetricRow1 = webIntegrationReferralsMetricRows.First(r => r.SessionId == "TestSession1");
                webIntegrationReferralsMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.001Z"));
                webIntegrationReferralsMetricRow1.Referrer.Should().Be("Int-ref1");
                webIntegrationReferralsMetricRow1.AuditId.Should().Be("AuditId1");
                webIntegrationReferralsMetricRow1.ReferrerOrigin.Should().Be("ref-Org1");

                var webIntegrationReferralsMetricRow2 = webIntegrationReferralsMetricRows.First(r => r.SessionId == "TestSession2");
                webIntegrationReferralsMetricRow2.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.002Z"));
                webIntegrationReferralsMetricRow2.Referrer.Should().Be("Int-ref2");
                webIntegrationReferralsMetricRow2.AuditId.Should().Be("AuditId2");
                webIntegrationReferralsMetricRow2.ReferrerOrigin.Should().Be(null);

                var consentMetricRow1 = consentMetricRows.First(r => r.SessionId == "TestSession3");
                consentMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.003Z"));
                consentMetricRow1.AuditId.Should().Be("AuditId3");

                var consentMetricRow2 = consentMetricRows.First(r => r.SessionId == "TestSession4");
                consentMetricRow2.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.004Z"));
                consentMetricRow2.AuditId.Should().Be("AuditId4");

                var consentMetricRow3 = consentMetricRows.First(r => r.SessionId == "TestSession5");
                consentMetricRow3.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.005Z"));
                consentMetricRow3.AuditId.Should().Be("AuditId5");

                var secondaryCareSummaryMetricRow1 = secondaryCareSummaryMetricRows.First(r => r.SessionId == "TestSession6");
                secondaryCareSummaryMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.006Z"));
                secondaryCareSummaryMetricRow1.TotalReferrals.Should().Be(123);
                secondaryCareSummaryMetricRow1.TotalUpcomingAppointments.Should().Be(456);
                secondaryCareSummaryMetricRow1.AuditId.Should().Be("AuditId6");

                var medicalRecordViewMetricRow1 = medicalRecordViewMetricRows.First(r => r.SessionId == "TestSession7");
                medicalRecordViewMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.007Z"));
                medicalRecordViewMetricRow1.HasSummaryRecordAccess.Should().Be(true);
                medicalRecordViewMetricRow1.HasDetailedRecordAccess.Should().Be(false);
                medicalRecordViewMetricRow1.AuditId.Should().Be("AuditId7");

                var notificationToggleMetricRow = notificationToggleMetricRows.First(r => r.LoginId == "NhsLoginSubject-Test");
                notificationToggleMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.008Z"));
                notificationToggleMetricRow.NotificationToggle.Should().Be("On");
                notificationToggleMetricRow.AuditId.Should().Be("AuditId8");

                var initialPromptMetricRow = initialPromptMetricRows.First(r => r.LoginId == "NhsLoginSubject-Test");
                initialPromptMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.007Z"));
                initialPromptMetricRow.OptedIn.Should().Be("On");
                initialPromptMetricRow.AuditId.Should().Be("AuditId9");

                var appointmentCancelMetricRow1 = appointmentCancelMetricRows.First(r => r.SessionId == "TestSession10");
                appointmentCancelMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.010Z"));
                appointmentCancelMetricRow1.AuditId.Should().Be("AuditId10");

                var appointmentBookMetricRow1 = appointmentBookMetricRows.First(r => r.SessionId == "TestSession11");
                appointmentBookMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.011Z"));
                appointmentBookMetricRow1.AuditId.Should().Be("AuditId11");
                appointmentBookMetricRow1.IsActingOnBehalfOfAnother.Should().Be(false);

                var organDonationRegistrationGetMetricRow1 = organDonationRegistrationGetMetricRows.First(r => r.SessionId == "TestSession12");
                organDonationRegistrationGetMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.012Z"));
                organDonationRegistrationGetMetricRow1.AuditId.Should().Be("AuditId12");

                var organDonationRegistrationCreateMetricRow1 = organDonationRegistrationCreateMetricRows.First(r => r.SessionId == "TestSession13");
                organDonationRegistrationCreateMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.013Z"));
                organDonationRegistrationCreateMetricRow1.AuditId.Should().Be("AuditId13");

                var repeatPrescriptionMetricRow1 = repeatPrescriptionMetricRows.First(r => r.SessionId == "TestSession14");
                repeatPrescriptionMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.014Z"));
                repeatPrescriptionMetricRow1.AuditId.Should().Be("AuditId14");

                var organDonationRegistrationWithdrawMetricRow1 = organDonationRegistrationWithdrawMetricRows.First(r => r.SessionId == "TestSession15");
                organDonationRegistrationWithdrawMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.015Z"));
                organDonationRegistrationWithdrawMetricRow1.AuditId.Should().Be("AuditId15");

                var organDonationRegistrationUpdateMetricRow1 = organDonationRegistrationUpdateMetricRows.First(r => r.SessionId == "TestSession16");
                organDonationRegistrationUpdateMetricRow1.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.016Z"));
                organDonationRegistrationUpdateMetricRow1.AuditId.Should().Be("AuditId16");

                var biometricsToggleMetricRow = biometricsToggleMetricRows.First(r => r.SessionId == "TestSession17");
                biometricsToggleMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.008Z"));
                biometricsToggleMetricRow.BiometricsToggle.Should().Be("On");
                biometricsToggleMetricRow.AuditId.Should().Be("AuditId17");

                var medicalRecordSectionViewMetricRow = medicalRecordSectionViewMetricRows.First(r => r.SessionId == "TestSession18");
                medicalRecordSectionViewMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.018Z"));
                medicalRecordSectionViewMetricRow.Supplier.Should().Be("Supplier-Test");
                medicalRecordSectionViewMetricRow.IsActingOnBehalfOfAnother.Should().Be(false);
                medicalRecordSectionViewMetricRow.Section.Should().Be("TEST RESULTS");
                medicalRecordSectionViewMetricRow.AuditId.Should().Be("AuditId18");
            }
        }

        [NhsAppTest]
        public async Task AuditLog_AuditLogEventHubMessages_WhenRowExistsInDB_DuplicatesNotLoaded(TestEnv env)
        {
            await env.Postgres.Events.LoginMetric.Insert(new LoginMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 1),
                OdsCode = "ods1",
                LoginId = "NhsLoginSubject-Test",
                ProofLevel = "P5",
                Referrer = "ref1",
                SessionId = "TestSession1",
                AuditId = "AuditId1"
            });

            await env.Postgres.Events.ConsentMetric.Insert(new ConsentMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 2),
                SessionId = "TestSession2",
                AuditId = "AuditId2",
                LoginId = "ConsentMetric-Test",
                ProofLevel = "P5"
            });

            await env.Postgres.Events.SecondaryCareSummaryMetric.Insert(new SecondaryCareSummaryMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 3),
                SessionId = "TestSession3",
                TotalReferrals = 123,
                TotalUpcomingAppointments = 456,
                AuditId = "AuditId3"
            });

            await env.Postgres.Events.MedicalRecordViewMetric.Insert(new MedicalRecordViewMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 4),
                SessionId = "TestSession4",
                HasSummaryRecordAccess = true,
                HasDetailedRecordAccess = false,
                AuditId = "AuditId4"
            });

            await env.Postgres.Events.NotificationToggleMetric.Insert(new NotificationToggleMetricRow
            {
                LoginId = "NhsLoginSubject-Test",
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 5),
                NotificationToggle = "On",
                AuditId = "AuditId5"
            });

            await env.Postgres.Events.InitialPromptMetric.Insert(new InitialPromptMetricRow
            {
                LoginId = "NhsLoginSubject-Test",
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 4),
                OptedIn = "On",
                AuditId = "AuditId6"
            });

            await env.Postgres.Events.AppointmentCancelMetric.Insert(new AppointmentCancelMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 7),
                SessionId = "TestSession7",
                AuditId = "AuditId7"
            });

            await env.Postgres.Events.AppointmentBookMetric.Insert(new AppointmentBookMetricRow() {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 8),
                SessionId = "TestSession8",
                AuditId = "AuditId8",
                IsActingOnBehalfOfAnother = false
            });

            await env.Postgres.Events.OrganDonationRegistrationGetMetric.Insert(new OrganDonationRegistrationGetMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 9),
                SessionId = "TestSession9",
                AuditId = "AuditId9"
            });

            await env.Postgres.Events.OrganDonationRegistrationCreateMetric.Insert(new OrganDonationRegistrationCreateMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 10),
                SessionId = "TestSession10",
                AuditId = "AuditId10"
            });

            await env.Postgres.Events.PrescriptionOrdersMetric.Insert(new PrescriptionOrdersMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 11),
                SessionId = "TestSession11",
                AuditId = "AuditId11"
            });

            await env.Postgres.Events.OrganDonationRegistrationWithdrawMetric.Insert(new OrganDonationRegistrationWithdrawMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 12),
                SessionId = "TestSession12",
                AuditId = "AuditId12"
            });

            await env.Postgres.Events.OrganDonationRegistrationUpdateMetric.Insert(new OrganDonationRegistrationUpdateMetricRow()
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 13),
                SessionId = "TestSession13",
                AuditId = "AuditId13"
            });

            await env.Postgres.Events.BiometricsToggleMetric.Insert(new BiometricsToggleMetricRow
            {
                SessionId = "TestSession17",
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 5),
                BiometricsToggle = "On",
                AuditId = "AuditId17"
            });

            await env.Postgres.Events.MedicalRecordSectionViewMetric.Insert(new MedicalRecordSectionViewMetricRow
            {
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 15),
                SessionId = "TestSession15",
                Supplier = "Test Supplier",
                IsActingOnBehalfOfAnother = true,
                Section = "TEST RESULT",
                AuditId = "AuditId15"
            });

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1),
                    "Login_Success",
                    "Successful Login with", "P5", "ods1", "ref-Org1", "ref1"),
                BuildEvent("AuditId2", "TestSession2", new DateTime(2021, 11, 01, 09, 00, 00, 2),
                    "TermsAndConditions_RecordConsent_Response",
                    "Initial Consent Successfully recorded", "P5", "ods2", "ref-Org2", "ref2"),
                BuildEvent("AuditId3", "TestSession3", new DateTime(2021, 11, 01, 09, 00, 00, 3),
                    "SecondaryCare_GetSummary_Response",
                    "Secondary Care Summary successfully retrieved. Total Referrals: 123, Total Upcoming Appointments: 456",
                    "P9", "ods3", "ref-Org3", "ref3"),
                BuildEvent("AuditId4", "TestSession4", new DateTime(2021, 11, 01, 09, 00, 00, 4),
                    "PatientRecord_View_Response",
                    "Patient record successfully retrieved. hasSummaryRecordAccess=True, hasDetailedRecordAccess=False",
                    "P9", "ods4", "ref-Org4", "ref4"),
                BuildEvent("AuditId5", "TestSession5", new DateTime(2021, 11, 01, 09, 00, 00, 5),
                    "NotificationToggle_Response", "Notification toggled. optIn=true", "P5", "ods5", "ref-Org5", "ref5"),
                BuildEvent("AuditId6", "TestSession6", new DateTime(2021, 11, 01, 09, 00, 00, 6),
                    "InitialNotificationPrompt_Decision", "Initial notification prompt decision made. optIn=true", "P5",
                    "ods4", "ref-Org6", "ref4"),
                BuildEvent("AuditId7", "TestSession7", new DateTime(2021, 11, 01, 09, 00, 00, 7),
                    "Appointments_Cancel_Response",
                    "Appointment successfully cancelled for appointment with id: 237710", "P9", "ods7", "ref-Org7", "ref7"),
                BuildEvent("AuditId8", "TestSession8", new DateTime(2021, 11, 01, 09, 00, 00, 8),
                    "Appointments_Book_Response", "Appointment successfully booked for appointment with id: 237710",
                    "P9", "ods8", "ref-Org8", "ref8"),
                BuildEvent("AuditId9", "TestSession9", new DateTime(2021, 11, 01, 09, 00, 00, 9),
                    "OrganDonation_Get_Response", "A default organ donation registration has been generated", "P9",
                    "ods9", "ref-Org9", "ref9"),
                BuildEvent("AuditId10", "TestSession10", new DateTime(2021, 11, 01, 09, 00, 00, 10),
                    "OrganDonation_Registration_Response",
                    "The organ donation decision has been successfully registered", "P10", "ods10", "ref-Org10", "ref10"),
                BuildEvent("AuditId11", "TestSession11", new DateTime(2021, 11, 01, 09, 00, 00, 11),
                    "RepeatPrescriptions_OrderRepeatMedications_Response",
                    "Repeat prescription request successfully created with course ids: FakeCourse1", "P9", "ods11", "ref-Org11", "ref11"),
                BuildEvent("AuditId12", "TestSession12", new DateTime(2021, 11, 01, 09, 00, 00, 12),
                    "OrganDonation_Withdraw_Response", "The organ donation decision has been successfully Withdrawn",
                    "P9", "ods12", "ref-Org12", "ref12"),
                BuildEvent("AuditId13", "TestSession13", new DateTime(2021, 11, 01, 09, 00, 00, 13),
                    "OrganDonation_Update_Response", "The organ donation decision has been successfully updated", "P9",
                    "ods13", "ref-Org13", "ref13"),
                BuildEvent("AuditId17", "TestSession17", new DateTime(2021, 11, 01, 09, 00, 00, 5),
                    "BiometricsRegistration_Decision", "Biometrics toggled. optIn=true", "P5", "ods17", "ref-Org17", "ref17"),
                BuildEvent("AuditId15", "TestSession15", new DateTime(2021, 11, 01, 09, 00, 00, 15),
                    "PatientRecord_Section_View_Response", "Patient record TEST RESULTS successfully retrieved.", "P9", "ods15", "ref-Org15", "ref15"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginMetricRows = await env.Postgres.Events.LoginMetric.FetchAll();
            var loginMetricRow = loginMetricRows.Should().ContainSingle().Subject;

            var webIntegrationReferralsMetricRows = await env.Postgres.Events.WebIntegrationReferrals.FetchAll();
            var webIntegrationReferralsMetricRow = webIntegrationReferralsMetricRows.Should().ContainSingle().Subject;

            var consentMetricRows = await env.Postgres.Events.ConsentMetric.FetchAll();
            var consentMetricRow = consentMetricRows.Should().ContainSingle().Subject;

            var secondaryCareSummaryMetricRows = await env.Postgres.Events.SecondaryCareSummaryMetric.FetchAll();
            var secondaryCareSummaryMetricRow = secondaryCareSummaryMetricRows.Should().ContainSingle().Subject;

            var medicalRecordViewMetricRows = await env.Postgres.Events.MedicalRecordViewMetric.FetchAll();
            var medicalRecordViewMetricRow = medicalRecordViewMetricRows.Should().ContainSingle().Subject;

            var notificationToggleMetricRows = await env.Postgres.Events.NotificationToggleMetric.FetchAll();
            var notificationToggleMetricRow = notificationToggleMetricRows.Should().ContainSingle().Subject;

            var initialPromptMetricRows = await env.Postgres.Events.InitialPromptMetric.FetchAll();
            var initialPromptMetricRow = initialPromptMetricRows.Should().ContainSingle().Subject;

            var appointmentCancelMetricRows = await env.Postgres.Events.AppointmentCancelMetric.FetchAll();
            var appointmentCancelMetricRow = appointmentCancelMetricRows.Should().ContainSingle().Subject;

            var appointmentBookMetricRows = await env.Postgres.Events.AppointmentBookMetric.FetchAll();
            var appointmentBookMetricRow = appointmentBookMetricRows.Should().ContainSingle().Subject;

            var organDonationRegistrationGetMetricRows = await env.Postgres.Events.OrganDonationRegistrationGetMetric.FetchAll();
            var organDonationRegistrationGetMetricRow = organDonationRegistrationGetMetricRows.Should().ContainSingle().Subject;

            var organDonationRegistrationCreateMetricRows = await env.Postgres.Events.OrganDonationRegistrationCreateMetric.FetchAll();
            var organDonationRegistrationCreateMetricRow = organDonationRegistrationCreateMetricRows.Should().ContainSingle().Subject;

            var repeatPrescriptionMetricRows = await env.Postgres.Events.PrescriptionOrdersMetric.FetchAll();
            var repeatPrescriptionMetricRow = repeatPrescriptionMetricRows.Should().ContainSingle().Subject;

            var organDonationRegistrationWithdrawMetricRows = await env.Postgres.Events.OrganDonationRegistrationWithdrawMetric.FetchAll();
            var organDonationRegistrationWithdrawMetricRow = organDonationRegistrationWithdrawMetricRows.Should().ContainSingle().Subject;

            var organDonationRegistrationUpdateMetricRows = await env.Postgres.Events.OrganDonationRegistrationUpdateMetric.FetchAll();
            var organDonationRegistrationUpdateMetricRow = organDonationRegistrationUpdateMetricRows.Should().ContainSingle().Subject;

            var biometricsToggleMetricRows = await env.Postgres.Events.BiometricsToggleMetric.FetchAll();
            var biometricsToggleMetricRow = biometricsToggleMetricRows.Should().ContainSingle().Subject;

            var medicalRecordSectionViewMetricRows = await env.Postgres.Events.MedicalRecordSectionViewMetric.FetchAll();
            var medicalRecordSectionViewMetricRow = medicalRecordSectionViewMetricRows.Should().ContainSingle().Subject;

            using (new AssertionScope())
            {
                loginMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.001Z"));
                loginMetricRow.OdsCode.Should().Be("ods1");
                loginMetricRow.LoginId.Should().Be("NhsLoginSubject-Test");
                loginMetricRow.ProofLevel.Should().Be("P5");
                loginMetricRow.Referrer.Should().Be("ref1");
                loginMetricRow.SessionId.Should().Be("TestSession1");
                loginMetricRow.AuditId.Should().Be("AuditId1");

                webIntegrationReferralsMetricRow.SessionId.Should().Be("TestSession1");
                webIntegrationReferralsMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.001Z"));
                webIntegrationReferralsMetricRow.Referrer.Should().Be("Int-ref1");
                webIntegrationReferralsMetricRow.AuditId.Should().Be("AuditId1");
                webIntegrationReferralsMetricRow.ReferrerOrigin.Should().Be("ref-Org1");

                consentMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.002Z"));
                consentMetricRow.SessionId.Should().Be("TestSession2");
                consentMetricRow.AuditId.Should().Be("AuditId2");
                consentMetricRow.ProofLevel.Should().Be("P5");
                consentMetricRow.LoginId.Should().Be("ConsentMetric-Test");

                secondaryCareSummaryMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.003Z"));
                secondaryCareSummaryMetricRow.SessionId.Should().Be("TestSession3");
                secondaryCareSummaryMetricRow.TotalReferrals.Should().Be(123);
                secondaryCareSummaryMetricRow.TotalUpcomingAppointments.Should().Be(456);
                secondaryCareSummaryMetricRow.AuditId.Should().Be("AuditId3");

                medicalRecordViewMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.004Z"));
                medicalRecordViewMetricRow.SessionId.Should().Be("TestSession4");
                medicalRecordViewMetricRow.HasSummaryRecordAccess.Should().Be(true);
                medicalRecordViewMetricRow.HasDetailedRecordAccess.Should().Be(false);
                medicalRecordViewMetricRow.AuditId.Should().Be("AuditId4");

                notificationToggleMetricRow.LoginId.Should().Be("NhsLoginSubject-Test");
                notificationToggleMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.005Z"));
                notificationToggleMetricRow.NotificationToggle.Should().Be("On");
                notificationToggleMetricRow.AuditId.Should().Be("AuditId5");

                initialPromptMetricRow.LoginId.Should().Be("NhsLoginSubject-Test");
                initialPromptMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.004Z"));
                initialPromptMetricRow.OptedIn.Should().Be("On");
                initialPromptMetricRow.AuditId.Should().Be("AuditId6");

                appointmentCancelMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.007Z"));
                appointmentCancelMetricRow.SessionId.Should().Be("TestSession7");
                appointmentCancelMetricRow.AuditId.Should().Be("AuditId7");

                appointmentBookMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.008Z"));
                appointmentBookMetricRow.SessionId.Should().Be("TestSession8");
                appointmentBookMetricRow.AuditId.Should().Be("AuditId8");
                appointmentBookMetricRow.IsActingOnBehalfOfAnother.Should().Be(false);

                organDonationRegistrationGetMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.009Z"));
                organDonationRegistrationGetMetricRow.SessionId.Should().Be("TestSession9");
                organDonationRegistrationGetMetricRow.AuditId.Should().Be("AuditId9");

                organDonationRegistrationCreateMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.010Z"));
                organDonationRegistrationCreateMetricRow.SessionId.Should().Be("TestSession10");
                organDonationRegistrationCreateMetricRow.AuditId.Should().Be("AuditId10");

                repeatPrescriptionMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.011Z"));
                repeatPrescriptionMetricRow.SessionId.Should().Be("TestSession11");
                repeatPrescriptionMetricRow.AuditId.Should().Be("AuditId11");

                organDonationRegistrationWithdrawMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.012Z"));
                organDonationRegistrationWithdrawMetricRow.SessionId.Should().Be("TestSession12");
                organDonationRegistrationWithdrawMetricRow.AuditId.Should().Be("AuditId12");

                organDonationRegistrationUpdateMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.013Z"));
                organDonationRegistrationUpdateMetricRow.SessionId.Should().Be("TestSession13");
                organDonationRegistrationUpdateMetricRow.AuditId.Should().Be("AuditId13");

                biometricsToggleMetricRow.SessionId.Should().Be("TestSession17");
                biometricsToggleMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.005Z"));
                biometricsToggleMetricRow.BiometricsToggle.Should().Be("On");
                biometricsToggleMetricRow.AuditId.Should().Be("AuditId17");

                medicalRecordSectionViewMetricRow.SessionId.Should().Be("TestSession15");
                medicalRecordSectionViewMetricRow.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.015Z"));
                medicalRecordSectionViewMetricRow.Supplier.Should().Be("Test Supplier");
                medicalRecordSectionViewMetricRow.IsActingOnBehalfOfAnother.Should().Be(true);
                medicalRecordSectionViewMetricRow.Section.Should().Be("TEST RESULT");
                medicalRecordSectionViewMetricRow.AuditId.Should().Be("AuditId15");
            }
        }

        [NhsAppTest]
        public async Task AuditLog_AuditLogEventHubMessagesWithInvalidOperationField_AreNotLoaded(TestEnv env)
        {
            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1), "Test_Request", "Successful Login with", "P5", "ods1", "ref-Org1", "ref1"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginMetricRows = await env.Postgres.Events.LoginMetric.FetchAll();
            loginMetricRows.Should().BeEmpty();

            var webIntegrationReferralsMetricRows = await env.Postgres.Events.WebIntegrationReferrals.FetchAll();
            webIntegrationReferralsMetricRows.Should().BeEmpty();

            var consentMetricRows = await env.Postgres.Events.ConsentMetric.FetchAll();
            consentMetricRows.Should().BeEmpty();

            var secondaryCareSummaryMetricRows = await env.Postgres.Events.SecondaryCareSummaryMetric.FetchAll();
            secondaryCareSummaryMetricRows.Should().BeEmpty();

            var medicalRecordViewMetricRows = await env.Postgres.Events.MedicalRecordViewMetric.FetchAll();
            medicalRecordViewMetricRows.Should().BeEmpty();

            var notificationToggleMetricRows = await env.Postgres.Events.NotificationToggleMetric.FetchAll();
            notificationToggleMetricRows.Should().BeEmpty();

            var initialPromptMetricRows = await env.Postgres.Events.InitialPromptMetric.FetchAll();
            initialPromptMetricRows.Should().BeEmpty();

            var appointmentCancelMetricRows = await env.Postgres.Events.AppointmentCancelMetric.FetchAll();
            appointmentCancelMetricRows.Should().BeEmpty();

            var appointmentBookMetricRows = await env.Postgres.Events.AppointmentBookMetric.FetchAll();
            appointmentBookMetricRows.Should().BeEmpty();

            var organDonationRegistrationGetMetricRow = await env.Postgres.Events.OrganDonationRegistrationGetMetric.FetchAll();
            organDonationRegistrationGetMetricRow.Should().BeEmpty();

            var organDonationRegistrationCreateMetricRows = await env.Postgres.Events.OrganDonationRegistrationCreateMetric.FetchAll();
            organDonationRegistrationCreateMetricRows.Should().BeEmpty();

            var organDonationRegistrationWithdrawMetricRows = await env.Postgres.Events.OrganDonationRegistrationWithdrawMetric.FetchAll();
            organDonationRegistrationWithdrawMetricRows.Should().BeEmpty();

            var repeatPrescriptionMetricRows = await env.Postgres.Events.PrescriptionOrdersMetric.FetchAll();
            repeatPrescriptionMetricRows.Should().BeEmpty();

            var organDonationRegistrationUpdateMetricRows = await env.Postgres.Events.OrganDonationRegistrationUpdateMetric.FetchAll();
            organDonationRegistrationUpdateMetricRows.Should().BeEmpty();

            var biometricsToggleMetricRows = await env.Postgres.Events.BiometricsToggleMetric.FetchAll();
            biometricsToggleMetricRows.Should().BeEmpty();

            var medicalRecordSectionViewMetricRows = await env.Postgres.Events.MedicalRecordSectionViewMetric.FetchAll();
            medicalRecordSectionViewMetricRows.Should().BeEmpty();
        }

        [NhsAppTest]
        public async Task AuditLog_AuditLogEventHubMessagesWithInvalidDetailsField_AreNotLoaded(TestEnv env)
        {
            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1), "Login_Success", "Test Details", "P5", "ods1", "ref-Org1", "ref1"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginMetricRows = await env.Postgres.Events.LoginMetric.FetchAll();
            loginMetricRows.Should().BeEmpty();

            var webIntegrationReferralsMetricRows = await env.Postgres.Events.WebIntegrationReferrals.FetchAll();
            webIntegrationReferralsMetricRows.Should().BeEmpty();

            var consentMetricRows = await env.Postgres.Events.ConsentMetric.FetchAll();
            consentMetricRows.Should().BeEmpty();

            var secondaryCareSummaryMetricRows = await env.Postgres.Events.SecondaryCareSummaryMetric.FetchAll();
            secondaryCareSummaryMetricRows.Should().BeEmpty();

            var medicalRecordViewMetricRows = await env.Postgres.Events.MedicalRecordViewMetric.FetchAll();
            medicalRecordViewMetricRows.Should().BeEmpty();

            var notificationToggleMetricRows = await env.Postgres.Events.NotificationToggleMetric.FetchAll();
            notificationToggleMetricRows.Should().BeEmpty();

            var initialPromptMetricRows = await env.Postgres.Events.InitialPromptMetric.FetchAll();
            initialPromptMetricRows.Should().BeEmpty();

            var appointmentCancelMetricRows = await env.Postgres.Events.AppointmentCancelMetric.FetchAll();
            appointmentCancelMetricRows.Should().BeEmpty();

            var appointmentBookMetricRows = await env.Postgres.Events.AppointmentBookMetric.FetchAll();
            appointmentBookMetricRows.Should().BeEmpty();

            var organDonationRegistrationGetMetricRow = await env.Postgres.Events.OrganDonationRegistrationGetMetric.FetchAll();
            organDonationRegistrationGetMetricRow.Should().BeEmpty();

            var organDonationRegistrationCreateMetricRows = await env.Postgres.Events.OrganDonationRegistrationCreateMetric.FetchAll();
            organDonationRegistrationCreateMetricRows.Should().BeEmpty();

            var repeatPrescriptionMetricRows = await env.Postgres.Events.PrescriptionOrdersMetric.FetchAll();
            repeatPrescriptionMetricRows.Should().BeEmpty();

            var organDonationRegistrationWithdrawMetricRows = await env.Postgres.Events.OrganDonationRegistrationWithdrawMetric.FetchAll();
            organDonationRegistrationWithdrawMetricRows.Should().BeEmpty();

            var organDonationRegistrationUpdateMetricRows = await env.Postgres.Events.OrganDonationRegistrationUpdateMetric.FetchAll();
            organDonationRegistrationUpdateMetricRows.Should().BeEmpty();

            var biometricsToggleMetricRows = await env.Postgres.Events.BiometricsToggleMetric.FetchAll();
            biometricsToggleMetricRows.Should().BeEmpty();

            var medicalRecordSectionViewMetricRows = await env.Postgres.Events.MedicalRecordSectionViewMetric.FetchAll();
            medicalRecordSectionViewMetricRows.Should().BeEmpty();
        }

        private static AuditRecord BuildEvent(string auditId, string sessionId, DateTime eventTimestamp, string operation, string details, string proofLevel, string odsCode, string referrerOrigin, string referrer)
        {
            var auditRecord = new AuditRecord()
            {
                AuditId = auditId,
                NhsLoginSubject = "NhsLoginSubject-Test",
                NhsNumber = "NhsNumber-Test",
                IsActingOnBehalfOfAnother = false,
                Supplier = "Supplier-Test",
                Operation = operation,
                Details = details,
                ApiVersion = "Api-Test",
                WebVersion = "Web-Test",
                NativeVersion = "NativeVersion-Test",
                Environment = "localtest",
                SessionId = sessionId,
                Timestamp = eventTimestamp,
                ProofLevel = proofLevel,
                ODS = odsCode,
                Referrer = referrer,
                ReferrerOrigin = referrerOrigin,
                IntegrationReferrer = $"Int-{referrer}"
            };

            return auditRecord;
        }

        private static AuditRecord[] BuildEvents() =>
            new[]
            {
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1), "Login_Success", "Successful Login with", "P5", "ods1", "ref-Org1", "ref1"),
                BuildEvent("AuditId2", "TestSession2", new DateTime(2021, 11, 01, 09, 00, 00, 2), "Login_Success", "Successful Login with", "P9", "ods2", null, "ref2"),
                BuildEvent("AuditId3", "TestSession3", new DateTime(2021, 11, 01, 09, 00, 00, 3), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded", "P5", "ods3", "ref-Org3", "ref3"),
                BuildEvent("AuditId4", "TestSession4", new DateTime(2021, 11, 01, 09, 00, 00, 4), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded", "P5", "ods4", "ref-Org4", "ref4"),
                BuildEvent("AuditId5", "TestSession5", new DateTime(2021, 11, 01, 09, 00, 00, 5), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded", "P5", "ods5", "ref-Org5", "ref5"),
                BuildEvent("AuditId6", "TestSession6", new DateTime(2021, 11, 01, 09, 00, 00, 6), "SecondaryCare_GetSummary_Response", "Secondary Care Summary successfully retrieved. Total Referrals: 123, Total Upcoming Appointments: 456", "P9", "ods6", "ref-Org6", "ref6"),
                BuildEvent("AuditId7", "TestSession7", new DateTime(2021, 11, 01, 09, 00, 00, 7), "PatientRecord_View_Response", "Patient record successfully retrieved. hasSummaryRecordAccess=True, hasDetailedRecordAccess=False", "P9", "ods7", "ref-Org7", "ref7"),
                BuildEvent("AuditId8", "TestSession8", new DateTime(2021, 11, 01, 09, 00, 00, 8), "NotificationToggle_Response", "Notification toggled. optIn=true", "P5", "ods8", "ref-Org8", "ref8"),
                BuildEvent("AuditId9", "TestSession9", new DateTime(2021, 11, 01, 09, 00, 00, 7), "InitialNotificationPrompt_Decision", "Initial notification prompt decision made. optIn=true", "P5", "ods7", "ref-Org7", "ref7"),
                BuildEvent("AuditId10", "TestSession10", new DateTime(2021, 11, 01, 09, 00, 00, 10), "Appointments_Cancel_Response", "Appointment successfully cancelled for appointment with id: 237710", "P9", "ods10", "refO-rg10", "ref10"),
                BuildEvent("AuditId11", "TestSession11", new DateTime(2021, 11, 01, 09, 00, 00, 11), "Appointments_Book_Response", "Appointment successfully booked for appointment with id: 237710", "P9", "ods11", "ref-Org11", "ref11"),
                BuildEvent("AuditId12", "TestSession12", new DateTime(2021, 11, 01, 09, 00, 00, 12), "OrganDonation_Get_Response", "A default organ donation registration has been generated", "P9", "ods12", "ref-Org12", "ref12"),
                BuildEvent("AuditId13", "TestSession13", new DateTime(2021, 11, 01, 09, 00, 00, 13), "OrganDonation_Registration_Response", "The organ donation decision has been successfully registered", "P9", "ods13", "ref-Org13", "ref13"),
                BuildEvent("AuditId14", "TestSession14", new DateTime(2021, 11, 01, 09, 00, 00, 14), "RepeatPrescriptions_OrderRepeatMedications_Response", "Repeat prescription request successfully created with course ids: FakeCourse1", "P9", "ods14", "ref-Org14", "ref14"),
                BuildEvent("AuditId15", "TestSession15", new DateTime(2021, 11, 01, 09, 00, 00, 15), "OrganDonation_Withdraw_Response", "The organ donation decision has been successfully Withdrawn", "P9", "ods15", "ref-Org15", "ref15"),
                BuildEvent("AuditId16", "TestSession16", new DateTime(2021, 11, 01, 09, 00, 00, 16), "OrganDonation_Update_Response", "The organ donation decision has been successfully updated", "P9", "ods16", "ref-Org16", "ref16"),
                BuildEvent("AuditId17", "TestSession17", new DateTime(2021, 11, 01, 09, 00, 00, 8), "BiometricsRegistration_Decision", "Biometrics toggled. optIn=True", "P5", "ods17", "ref-Org17", "ref17"),
                BuildEvent("AuditId18", "TestSession18", new DateTime(2021, 11, 01, 09, 00, 00, 18), "PatientRecord_Section_View_Response", "Patient record TEST RESULTS successfully retrieved.", "P9", "ods18", "ref-Org18", "ref18"),
            };
    }
}
