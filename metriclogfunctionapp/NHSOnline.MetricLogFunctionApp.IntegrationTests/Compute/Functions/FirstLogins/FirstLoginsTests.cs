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
    public class FirstLoginsTests
    {
        [NhsAppTest]
        public async Task MultipleRecords_InTimeRange_AreLoadedAndUpdated(TestEnv env)
        {
            const string loginId1 = "LoginId1";
            const string loginId2 = "LoginIn2";
            const string loginId3 = "LoginId3";
            const string loginId4 = "LoginId4";
            const string loginId5 = "LoginId5";
            const string loginId6 = "LoginId6";
            const string p5ProofLevel = "P5";
            const string p9ProofLevel = "P9";
            const string startTime = "2020-10-08T09:00:00";
            const string endTime = "2020-10-09T12:00:00";
            var defaultDateTime = new DateTime(2020, 10, 06, 09, 30, 00, DateTimeKind.Utc);

            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow()
            {
                LoginId = loginId2,
                FirstP5LoginDate = defaultDateTime,
                FirstP9LoginDate = null,
                ConsentDate = defaultDateTime,
                ConsentProofLevel = p9ProofLevel,
                FirstP5LoginTimestamp = defaultDateTime,
                FirstP9LoginTimestamp = null,
                ConsentTimestamp = defaultDateTime
            });
            await env.Postgres.Compute.FirstLogins.Insert(new FirstLoginsRow()
            {
                LoginId = loginId4,
                FirstP5LoginDate = defaultDateTime,
                FirstP9LoginDate = null,
                ConsentDate = null,
                ConsentProofLevel = p9ProofLevel,
                FirstP5LoginTimestamp = defaultDateTime,
                FirstP9LoginTimestamp = null,
                ConsentTimestamp = null
            });

            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId1", "SessionId", new DateTime(2020, 10, 08, 09, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", p5ProofLevel, "AB123", "ref1",loginId1),
                FirstLoginMetrics.BuildEvent("AuditId2", "SessionId", new DateTime(2020, 10, 08, 10, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", p9ProofLevel, "AB123", "ref1",loginId1),
                FirstLoginMetrics.BuildEvent("AuditId3", "SessionId", new DateTime(2020, 10, 08, 11, 30, 00, DateTimeKind.Utc), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    p9ProofLevel, "AB123", "ref2",loginId1),
                FirstLoginMetrics.BuildEvent("AuditId4", "SessionId", new DateTime(2020, 10, 08, 10, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", p9ProofLevel, "AB123", "ref1",loginId2),
                FirstLoginMetrics.BuildEvent("AuditId5", "SessionId", new DateTime(2020, 10, 08, 09, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", p5ProofLevel, "AB123", "ref1",loginId3),
                FirstLoginMetrics.BuildEvent("AuditId6", "SessionId", new DateTime(2020, 10, 08, 11, 30, 00, DateTimeKind.Utc), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    p5ProofLevel, "AB123", "ref2",loginId4),
                FirstLoginMetrics.BuildEvent("AuditId7", "SessionId", new DateTime(2020, 10, 07, 09, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", p5ProofLevel, "AB123", "ref1",loginId5),
                FirstLoginMetrics.BuildEvent("AuditId8", "SessionId", new DateTime(2020, 10, 07, 10, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", p9ProofLevel, "AB123", "ref1",loginId5),
                FirstLoginMetrics.BuildEvent("AuditId9", "SessionId", new DateTime(2020, 10, 07, 11, 30, 00, DateTimeKind.Utc), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    p9ProofLevel, "AB123", "ref2",loginId5),
                FirstLoginMetrics.BuildEvent("AuditId10", "SessionId", new DateTime(2020, 10, 10, 09, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", p5ProofLevel, "AB123", "ref1",loginId6),
                FirstLoginMetrics.BuildEvent("AuditId11", "SessionId", new DateTime(2020, 10, 10, 10, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", p9ProofLevel, "AB123", "ref1",loginId6),
                FirstLoginMetrics.BuildEvent("AuditId12", "SessionId", new DateTime(2020, 10, 10, 11, 30, 00, DateTimeKind.Utc), "TermsAndConditions_RecordConsent_Response", "Initial Consent Successfully recorded",
                    p9ProofLevel, "AB123", "ref2",loginId6),
            };

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();

            using (new AssertionScope())
            {
                rows.Count.Should().Be(6);

                var row1 = rows.Single(x => x.LoginId == loginId1);
                row1.FirstP5LoginDate.Should().Be(new DateTime(2020, 10, 08));
                row1.FirstP9LoginDate.Should().Be(new DateTime(2020, 10, 08));
                row1.ConsentDate.Should().Be(new DateTime(2020, 10, 08));
                row1.ConsentProofLevel.Should().Be(p9ProofLevel);
                row1.FirstP5LoginTimestamp.Should().Be( new DateTimeOffset(2020, 10, 08, 09, 30, 00, TimeSpan.Zero));
                row1.FirstP9LoginTimestamp.Should().Be(new DateTimeOffset(2020, 10, 08, 10, 30, 00, TimeSpan.Zero));
                row1.ConsentTimestamp.Should().Be(new DateTimeOffset(2020, 10, 08, 11, 30, 00, TimeSpan.Zero));

                var row2 = rows.Single(x => x.LoginId == loginId2);
                row2.FirstP5LoginDate.Should().Be(defaultDateTime.Date);
                row2.FirstP9LoginDate.Should().Be(new DateTime(2020, 10, 08));
                row2.ConsentDate.Should().Be(defaultDateTime.Date);
                row2.ConsentProofLevel.Should().Be(p9ProofLevel);
                row2.FirstP5LoginTimestamp.Should().Be(defaultDateTime);
                row2.FirstP9LoginTimestamp.Should().Be(new DateTimeOffset(2020, 10, 08, 10, 30, 00, TimeSpan.Zero));
                row2.ConsentTimestamp.Should().Be(defaultDateTime);

                var row3 = rows.Single(x => x.LoginId == loginId3);
                row3.FirstP5LoginDate.Should().Be(new DateTime(2020, 10, 08));
                row3.FirstP9LoginDate.Should().Be(null);
                row3.ConsentDate.Should().Be(null);
                row3.ConsentProofLevel.Should().Be(null);
                row3.FirstP5LoginTimestamp.Should().Be(new DateTimeOffset(2020, 10, 08, 09, 30, 00, TimeSpan.Zero));
                row3.FirstP9LoginTimestamp.Should().Be(null);
                row3.ConsentTimestamp.Should().Be(null);

                var row4 = rows.Single(x => x.LoginId == loginId4);
                row4.FirstP5LoginDate.Should().Be(defaultDateTime.Date);
                row4.FirstP9LoginDate.Should().Be(null);
                row4.ConsentDate.Should().Be(new DateTime(2020, 10, 08));
                row4.ConsentProofLevel.Should().Be(p5ProofLevel);
                row4.FirstP5LoginTimestamp.Should().Be(defaultDateTime);
                row4.FirstP9LoginTimestamp.Should().Be(null);
                row4.ConsentTimestamp.Should().Be(new DateTimeOffset(2020, 10, 08, 11, 30, 00, TimeSpan.Zero));
            }
        }
    }
}
