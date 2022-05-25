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

                var webIntegrationReferralsMetricRow2 = webIntegrationReferralsMetricRows.First(r => r.SessionId == "TestSession2");
                webIntegrationReferralsMetricRow2.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.002Z"));
                webIntegrationReferralsMetricRow2.Referrer.Should().Be("Int-ref2");
                webIntegrationReferralsMetricRow2.AuditId.Should().Be("AuditId2");

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

                var webIntegrationReferralsMetricRow2 = webIntegrationReferralsMetricRows.First(r => r.SessionId == "TestSession2");
                webIntegrationReferralsMetricRow2.Timestamp.Should().Be(DateTime.Parse("2021-11-01T09:00:00.002Z"));
                webIntegrationReferralsMetricRow2.Referrer.Should().Be("Int-ref2");
                webIntegrationReferralsMetricRow2.AuditId.Should().Be("AuditId2");

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

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1),
                    "Login_Success",
                    "Successful Login with", "P5", "ods1", "ref1"),
                BuildEvent("AuditId2", "TestSession2", new DateTime(2021, 11, 01, 09, 00, 00, 2),
                    "TermsAndConditions_RecordConsent_Response",
                    "Initial Consent Successfully recorded", "P5", "ods2", "ref2"),
                BuildEvent("AuditId3", "TestSession3", new DateTime(2021, 11, 01, 09, 00, 00, 3),
                    "SecondaryCare_GetSummary_Response", "Secondary Care Summary successfully retrieved. Total Referrals: 123, Total Upcoming Appointments: 456", "P9", "ods3", "ref3"),
                BuildEvent("AuditId4", "TestSession4", new DateTime(2021, 11, 01, 09, 00, 00, 4),
                    "PatientRecord_View_Response", "Patient record successfully retrieved. hasSummaryRecordAccess=True, hasDetailedRecordAccess=False", "P9", "ods4", "ref4"),
                BuildEvent("AuditId5", "TestSession5", new DateTime(2021, 11, 01, 09, 00, 00, 5),
                    "NotificationToggle_Response", "Notification toggled. optIn=true", "P5", "ods5", "ref5"));

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
            }
        }

        [NhsAppTest]
        public async Task AuditLog_AuditLogEventHubMessagesWithInvalidOperationField_AreNotLoaded(TestEnv env)
        {
            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1), "Test_Request", "Successful Login with", "P5", "ods1", "ref1"));

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
        }

        [NhsAppTest]
        public async Task AuditLog_AuditLogEventHubMessagesWithInvalidDetailsField_AreNotLoaded(TestEnv env)
        {
            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1), "Login_Success", "Test Details", "P5", "ods1", "ref1"));

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
        }

        private static AuditRecord BuildEvent(string auditId, string sessionId, DateTime eventTimestamp, string operation, string details, string proofLevel, string odsCode, string referrer)
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
                IntegrationReferrer = $"Int-{referrer}"
            };

            return auditRecord;
        }

        private static AuditRecord[] BuildEvents() =>
            new[]
            {
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1), "Login_Success", "Successful Login with", "P5", "ods1", "ref1"),
                BuildEvent("AuditId2", "TestSession2", new DateTime(2021, 11, 01, 09, 00, 00, 2), "Login_Success", "Successful Login with", "P9", "ods2", "ref2"),
                BuildEvent("AuditId3", "TestSession3", new DateTime(2021, 11, 01, 09, 00, 00, 3), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded", "P5", "ods3", "ref3"),
                BuildEvent("AuditId4", "TestSession4", new DateTime(2021, 11, 01, 09, 00, 00, 4), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded", "P5", "ods4", "ref4"),
                BuildEvent("AuditId5", "TestSession5", new DateTime(2021, 11, 01, 09, 00, 00, 5), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded", "P5", "ods5", "ref5"),
                BuildEvent("AuditId6", "TestSession6", new DateTime(2021, 11, 01, 09, 00, 00, 6), "SecondaryCare_GetSummary_Response", "Secondary Care Summary successfully retrieved. Total Referrals: 123, Total Upcoming Appointments: 456", "P9", "ods6", "ref6"),
                BuildEvent("AuditId7", "TestSession7", new DateTime(2021, 11, 01, 09, 00, 00, 7), "PatientRecord_View_Response", "Patient record successfully retrieved. hasSummaryRecordAccess=True, hasDetailedRecordAccess=False", "P9", "ods7", "ref7"),
                BuildEvent("AuditId8", "TestSession8", new DateTime(2021, 11, 01, 09, 00, 00, 8), "NotificationToggle_Response", "Notification toggled. optIn=true", "P5", "ods8", "ref8")
            };
    }
}

