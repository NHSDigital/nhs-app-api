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
    public class LastLoginsAnalysisTests
    {
        private const string LoginId = "LoginId1";
        private const string StartDateTime = "2020-10-08T09:00:00";
        private const string EndDateTime = "2020-10-09T12:00:00";
        private const string P5ProofLevel = "P5";
        private const string P9ProofLevel = "P9";
        private const string LatestOdsCode = "AB123";
        public static readonly DateTimeOffset MetricDateTimeOffset = new DateTimeOffset(2020, 10, 08, 09, 30, 00, TimeSpan.Zero);
        private const string SingleLoginFlagYes = "Y";
        private const string SingleLoginFlagNo = "N";

        [NhsAppTest]
        public async Task FirstLogins_LastLoginsAnalysisP9LoginWhereSingleLoginFlag_ShouldBeYes(TestEnv env)
        {
            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId1", "SessionId", MetricDateTimeOffset.DateTime, "Login_Success",
                    "Successful Login with", P9ProofLevel, "AB123", "ref1",LoginId)
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.LatestOdsCode.Should().Be(LatestOdsCode);
                row1.LatestProofLevel.Should().Be(P9ProofLevel);
                row1.LatestLoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.SingleLoginFlag.Should().Be(SingleLoginFlagYes);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_LastLoginsAnalysisP5LoginWhereSingleLoginFlag_ShouldBeYes(TestEnv env)
        {
            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId1", "SessionId", MetricDateTimeOffset.DateTime, "Login_Success",
                    "Successful Login with", P5ProofLevel, "AB123", "ref1",LoginId)
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.LatestOdsCode.Should().Be(LatestOdsCode);
                row1.LatestProofLevel.Should().Be(P5ProofLevel);
                row1.LatestLoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.SingleLoginFlag.Should().Be(SingleLoginFlagYes);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_LastLoginsAnalysisP5LoginWhereSingleLoginFlag_ShouldBeNo(TestEnv env)
        {
            var p5FirstLoginDateTime = new DateTime(2020, 10, 16, 09, 30, 00, DateTimeKind.Utc);
            var p9FirstLoginDateTime = new DateTime(2020, 10, 10, 09, 30, 00, DateTimeKind.Utc);
            var consentDateTime = new DateTime(2020, 10, 10, 09, 30, 01, DateTimeKind.Utc);

            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = LoginId,
                FirstP5LoginDate = p5FirstLoginDateTime.Date,
                FirstP9LoginDate = p9FirstLoginDateTime.Date,
                ConsentDate = consentDateTime.Date,
                ConsentProofLevel = P9ProofLevel,
                FirstP5LoginTimestamp = p5FirstLoginDateTime,
                FirstP9LoginTimestamp = p9FirstLoginDateTime,
                ConsentTimestamp = consentDateTime
            });

            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId1", "SessionId", MetricDateTimeOffset.DateTime, "Login_Success",
                    "Successful Login with", P5ProofLevel, "AB123", "ref1",LoginId)
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.LatestOdsCode.Should().Be(LatestOdsCode);
                row1.LatestProofLevel.Should().Be(P5ProofLevel);
                row1.LatestLoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.SingleLoginFlag.Should().Be(SingleLoginFlagNo);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_LastLoginsAnalysisP9LoginWhereSingleLoginFlag_ShouldBeNo(TestEnv env)
        {
            var p5FirstLoginDateTime = new DateTimeOffset(2020, 10, 10, 09, 30, 00, TimeSpan.Zero);
            var p9FirstLoginDateTime = new DateTimeOffset(2020, 10, 16, 09, 30, 00, TimeSpan.Zero);
            var consentDateTime = new DateTime(2020, 10, 10, 09, 30, 01, DateTimeKind.Utc);

            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = LoginId,
                FirstP5LoginDate = p5FirstLoginDateTime.Date,
                FirstP9LoginDate = p9FirstLoginDateTime.Date,
                ConsentDate = consentDateTime,
                ConsentProofLevel = P9ProofLevel,
                FirstP5LoginTimestamp = p5FirstLoginDateTime,
                FirstP9LoginTimestamp = p9FirstLoginDateTime,
                ConsentTimestamp = consentDateTime
            });

            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId1", "SessionId", MetricDateTimeOffset.DateTime, "Login_Success",
                    "Successful Login with", P9ProofLevel, "AB123", "ref1",LoginId)
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.LatestOdsCode.Should().Be(LatestOdsCode);
                row1.LatestProofLevel.Should().Be(P9ProofLevel);
                row1.LatestLoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.SingleLoginFlag.Should().Be(SingleLoginFlagNo);
            }
        }
    }
}