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
    public class FirstLoginsP5Tests
    {
        private const string P5ProofLevel = "P5";
        private const string P9ProofLevel = "P9";
        private const string LoginId = "LoginId1";
        public static readonly DateTimeOffset MetricDateTimeOffset = new DateTimeOffset(2020, 10, 08, 09, 30, 00, TimeSpan.Zero);

        private static async Task PostP5LoginEvent(TestEnv env)
        {
            var loginEvent = FirstLoginMetrics.BuildEvent(
                "AuditId1", "SessionId", MetricDateTimeOffset.DateTime,
                "Login_Success", "Successful Login with", P5ProofLevel, "AB123", "ref1", LoginId
            );

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(loginEvent);
            response.StatusCode.Should().Be(HttpStatusCode.OK);
        }

        [NhsAppTest]
        public async Task FirstLogins_P5Login_IsLoaded(TestEnv env)
        {
            await PostP5LoginEvent(env);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(MetricDateTimeOffset.Date);
                row1.FirstP9LoginDate.Should().Be(null);
                row1.ConsentDate.Should().Be(null);
                row1.ConsentProofLevel.Should().Be(null);
                row1.FirstP5LoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.FirstP9LoginTimestamp.Should().Be(null);
                row1.ConsentTimestamp.Should().Be(null);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_P5LoginWhereRecordExistsWithNullP5_IsUpdated(TestEnv env)
        {
            var firstLoginDateTime = new DateTime(2020, 10, 10, 09, 30, 00, DateTimeKind.Utc);
            var consentDateTime = new DateTime(2020, 10, 10, 09, 30, 01, DateTimeKind.Utc);

            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow
            {
                LoginId = LoginId,
                FirstP5LoginDate = null,
                FirstP9LoginDate = firstLoginDateTime.Date,
                ConsentDate = consentDateTime.Date,
                ConsentProofLevel = P9ProofLevel,
                FirstP5LoginTimestamp = null,
                FirstP9LoginTimestamp = firstLoginDateTime,
                ConsentTimestamp = consentDateTime
            });

            await PostP5LoginEvent(env);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(MetricDateTimeOffset.Date);
                row1.FirstP9LoginDate.Should().Be(firstLoginDateTime.Date);
                row1.ConsentDate.Should().Be(consentDateTime.Date);
                row1.ConsentProofLevel.Should().Be(P9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.FirstP9LoginTimestamp.Should().Be(firstLoginDateTime);
                row1.ConsentTimestamp.Should().Be(consentDateTime);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_P5LoginWhereRecordExistsWithEarlierP5_IsNotUpdated(TestEnv env)
        {
            var p5FirstLoginDateTime = new DateTime(2020, 10, 6, 09, 30, 00, DateTimeKind.Utc);
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

            await PostP5LoginEvent(env);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(p5FirstLoginDateTime.Date);
                row1.FirstP9LoginDate.Should().Be(p9FirstLoginDateTime.Date);
                row1.ConsentDate.Should().Be(consentDateTime.Date);
                row1.ConsentProofLevel.Should().Be(P9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(p5FirstLoginDateTime);
                row1.FirstP9LoginTimestamp.Should().Be(p9FirstLoginDateTime);
                row1.ConsentTimestamp.Should().Be(consentDateTime);
            }
        }

        [NhsAppTest]
        public async Task FirstLogins_P5LoginWhereRecordExistsWithLaterP5_IsUpdated(TestEnv env)
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

            await PostP5LoginEvent(env);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(1);
                var row1 = rows.Single(x => x.LoginId == LoginId);
                row1.FirstP5LoginDate.Should().Be(MetricDateTimeOffset.Date);
                row1.FirstP9LoginDate.Should().Be(p9FirstLoginDateTime.Date);
                row1.ConsentDate.Should().Be(consentDateTime.Date);
                row1.ConsentProofLevel.Should().Be(P9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be(MetricDateTimeOffset);
                row1.FirstP9LoginTimestamp.Should().Be(p9FirstLoginDateTime);
                row1.ConsentTimestamp.Should().Be(consentDateTime);
            }
        }
    }
}
