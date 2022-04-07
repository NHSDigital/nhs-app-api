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

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.FirstLogins
{
    [TestClass]
    public class FirstLoginsConsentTests
    {
        private const string P5ProofLevel = "P5";
        private const string P9ProofLevel = "P9";
        private const string LoginId = "LoginId1";
        private const string StartDateTime = "2020-10-08T09:00:00";
        private const string EndDateTime = "2020-10-09T12:00:00";
        public static readonly DateTimeOffset MetricDateTimeOffset = new DateTimeOffset(2020, 10, 08, 09, 30, 00, TimeSpan.Zero);

        [NhsAppTest]
        public async Task FirstLogins_ConsentLogin_IsLoaded(TestEnv env)
        {
            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId1", "SessionId", MetricDateTimeOffset.DateTime, "Login_Success",
                    "Successful Login with", P9ProofLevel, "AB123", "ref1",LoginId),
                FirstLoginMetrics.BuildEvent("AuditId2", "SessionId", MetricDateTimeOffset.DateTime, "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    P9ProofLevel, "AB123", "ref2",LoginId)
            };
            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(null);
                row1.FirstP9LoginDate.Should().Be(MetricDateTimeOffset.Date);
                row1.ConsentDate.Should().Be(MetricDateTimeOffset.Date);
                row1.ConsentProofLevel.Should().Be(P9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(null);
                row1.FirstP9LoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.ConsentTimestamp.Should().Be(MetricDateTimeOffset);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_ConsentLoginWhereRecordExistsWithNullConsent_IsUpdated(TestEnv env)
        {
            var firstLoginDateTime = new DateTime(2020, 10, 10, 09, 30, 00, DateTimeKind.Utc);

            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = LoginId,
                FirstP5LoginDate = firstLoginDateTime.Date,
                FirstP9LoginDate = firstLoginDateTime.Date,
                ConsentDate = null,
                ConsentProofLevel = null,
                FirstP5LoginTimestamp = firstLoginDateTime,
                FirstP9LoginTimestamp = firstLoginDateTime,
                ConsentTimestamp = null
            });
            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId2", "SessionId", MetricDateTimeOffset.DateTime, "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    P9ProofLevel, "AB123", "ref2",LoginId)
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(firstLoginDateTime.Date);
                row1.FirstP9LoginDate.Should().Be(firstLoginDateTime.Date);
                row1.ConsentDate.Should().Be(MetricDateTimeOffset.Date);
                row1.ConsentProofLevel.Should().Be(P9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(firstLoginDateTime);
                row1.FirstP9LoginTimestamp.Should().Be(firstLoginDateTime);
                row1.ConsentTimestamp.Should().Be(MetricDateTimeOffset);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_ConsentLoginWhereMissingFirstLogin_IsLoaded(TestEnv env)
        {
            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId2", "SessionId", MetricDateTimeOffset.DateTime, "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    P9ProofLevel, "AB123", "ref2",LoginId)
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(null);
                row1.FirstP9LoginDate.Should().Be(MetricDateTimeOffset.Date);
                row1.ConsentDate.Should().Be(MetricDateTimeOffset.Date);
                row1.ConsentProofLevel.Should().Be(P9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(null);
                row1.FirstP9LoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.ConsentTimestamp.Should().Be(MetricDateTimeOffset);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_ConsentLoginWhereConsentIsGivenTwiceToTheSameUserAtDifferentProofLevels_P5ProofLevelIsRecorded(TestEnv env)
        {
            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId1", "SessionId", MetricDateTimeOffset.DateTime, "Login_Success",
                    "Successful Login with", P5ProofLevel, "AB123", "ref1",LoginId),
                FirstLoginMetrics.BuildEvent("AuditId2", "SessionId", MetricDateTimeOffset.DateTime.AddSeconds(1), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    P5ProofLevel, "AB123", "ref2",LoginId),
                FirstLoginMetrics.BuildEvent("AuditId3", "SessionId", MetricDateTimeOffset.DateTime.AddSeconds(2), "Login_Success",
                    "Successful Login with", P9ProofLevel, "AB123", "ref1",LoginId),
                FirstLoginMetrics.BuildEvent("AuditId4", "SessionId", MetricDateTimeOffset.DateTime.AddSeconds(3), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    P9ProofLevel, "AB123", "ref2",LoginId)
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(MetricDateTimeOffset.Date);
                row1.FirstP9LoginDate.Should().Be(MetricDateTimeOffset.Date);
                row1.ConsentDate.Should().Be(MetricDateTimeOffset.Date);
                row1.ConsentProofLevel.Should().Be(P5ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.FirstP9LoginTimestamp.Should().Be(MetricDateTimeOffset.AddSeconds(2));
                row1.ConsentTimestamp.Should().Be(MetricDateTimeOffset.AddSeconds(1));
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_ConsentLoginWhereRecordExistsWithEarlierConsent_IsNotUpdated(TestEnv env)
        {
            var firstLoginDateTime = new DateTime(2020, 10, 10, 09, 30, 00,DateTimeKind.Utc);
            var consentDateTime = new DateTime(2020, 10, 06, 09, 30, 01,DateTimeKind.Utc);

            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = LoginId,
                FirstP5LoginDate = firstLoginDateTime.Date,
                FirstP9LoginDate = firstLoginDateTime.Date,
                ConsentDate = consentDateTime.Date,
                ConsentProofLevel = P9ProofLevel,
                FirstP5LoginTimestamp = firstLoginDateTime,
                FirstP9LoginTimestamp = firstLoginDateTime,
                ConsentTimestamp = consentDateTime
            });

            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId2", "SessionId", MetricDateTimeOffset.DateTime, "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    "P5", "AB123", "ref2","LoginId1")
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(firstLoginDateTime.Date);
                row1.FirstP9LoginDate.Should().Be(firstLoginDateTime.Date);
                row1.ConsentDate.Should().Be(consentDateTime.Date);
                row1.ConsentProofLevel.Should().Be(P9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(firstLoginDateTime);
                row1.FirstP9LoginTimestamp.Should().Be(firstLoginDateTime);
                row1.ConsentTimestamp.Should().Be(consentDateTime);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_ConsentLoginWhereRecordExistsWithLaterConsent_IsUpdated(TestEnv env)
        {
            var firstLoginDateTime = new DateTime(2020, 10, 10, 09, 30, 00, DateTimeKind.Utc);
            var p5ConsentDate = new DateTime(2020, 10, 16,00, 00, 00, DateTimeKind.Utc);

            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = LoginId,
                FirstP5LoginDate = firstLoginDateTime.Date,
                FirstP9LoginDate = firstLoginDateTime.Date,
                ConsentDate = p5ConsentDate,
                ConsentProofLevel = P5ProofLevel,
                FirstP5LoginTimestamp = firstLoginDateTime,
                FirstP9LoginTimestamp = firstLoginDateTime,
                ConsentTimestamp = p5ConsentDate
            });

            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId2", "SessionId", MetricDateTimeOffset.DateTime, "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    P9ProofLevel, "AB123", "ref2",LoginId)
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(firstLoginDateTime.Date);
                row1.FirstP9LoginDate.Should().Be(firstLoginDateTime.Date);
                row1.ConsentDate.Should().Be(MetricDateTimeOffset.Date);
                row1.ConsentProofLevel.Should().Be(P9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(firstLoginDateTime);
                row1.FirstP9LoginTimestamp.Should().Be(firstLoginDateTime);
                row1.ConsentTimestamp.Should().Be(MetricDateTimeOffset);
            }
        }
    }
}
