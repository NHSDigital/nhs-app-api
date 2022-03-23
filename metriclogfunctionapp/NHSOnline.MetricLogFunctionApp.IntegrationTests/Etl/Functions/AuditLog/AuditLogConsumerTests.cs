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

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(
                BuildEvent("AuditId1", "TestSession1", new DateTime(2021, 11, 01, 09, 00, 00, 1),
                    "Login_Success",
                    "Successful Login with", "P5", "ods1", "ref1"),
                BuildEvent("AuditId2", "TestSession2", new DateTime(2021, 11, 01, 09, 00, 00, 2),
                    "TermsAndConditions_RecordConsent_Response",
                    "Initial Consent Successfully recorded", "P5", "ods2", "ref2"));

            response.StatusCode.Should().Be(HttpStatusCode.OK);

            var loginMetricRows = await env.Postgres.Events.LoginMetric.FetchAll();
            var loginMetricRow = loginMetricRows.Should().ContainSingle().Subject;

            var webIntegrationReferralsMetricRows = await env.Postgres.Events.WebIntegrationReferrals.FetchAll();
            var webIntegrationReferralsMetricRow = webIntegrationReferralsMetricRows.Should().ContainSingle().Subject;

            var consentMetricRows = await env.Postgres.Events.ConsentMetric.FetchAll();
            var consentMetricRow = consentMetricRows.Should().ContainSingle().Subject;

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
                BuildEvent("AuditId5", "TestSession5", new DateTime(2021, 11, 01, 09, 00, 00, 5), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded", "P5", "ods5", "ref5")
            };
    }
}

