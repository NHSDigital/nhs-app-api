using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Storage;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.ReferrerLogin
{
    [TestClass]
    public class ReferrerLoginResilienceTests
    {
        [NhsAppTest]
        public async Task ReferrerLogin_InvalidRequestJson_FailsFast(TestEnv env)
        {
            var response = await env.HttpEndpointCallers.ReferrerLogin.PostJson(new { StartDateTime = "2020-10-08T09:00:00", EndDateTime = "Blah" });
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();
            rows.Should().BeEmpty("the data should not have been inserted");

            var poisonMessages = await env.Queues.ReferrerLogin.Poison.FetchAll();
            var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

            poisonMessage.AsJson().Should().BeEquivalentTo(
                new JObject(
                        new JProperty("StartDateTime", "2020-10-08T09:00:00"),
                        new JProperty("EndDateTime", "Blah"))
                );
        }

        [NhsAppTest]
        public async Task ReferrerLogin_InsertDataFails_RetriesAndFails(TestEnv env)
        {
            var startTime = "2022-05-17T00:00:00";
            var endTime = "2022-05-18T00:00:00";
            const string loginId1 = "LoginId1";
            const string p9ProofLevel = "P9";
            const string sessionId1 = "SessionId1";
            const string referrerId = "ReferrerFailure";

            await ReferrerLoginMetrics.AddLoginMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
            await ReferrerLoginMetrics.AddConsentMetric(env, loginId1, p9ProofLevel, new DateTimeOffset(2022, 05, 17, 11, 30, 00, TimeSpan.Zero), sessionId1);
            await ReferrerLoginMetrics.AddWebIntegrationReferralsMetric(env,
                new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, sessionId1);

            await env.Postgres.Compute.ReferrerLogin.SetupTrigger(@"
    IF NEW.""ReferrerId"" = 'ReferrerFailure' THEN
        RAISE EXCEPTION 'Simulating failure to insert with ReferrerId %', NEW.""ReferrerId"" USING ERRCODE = 'integrity_constraint_violation';
    END IF;
");

            var response = await env.HttpEndpointCallers.PostReferrerLogin(startTime, endTime);
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            await env.Queues.ReferrerLogin.WaitUntilEmpty();

            var functionLogs = env.FunctionLogs();
            functionLogs.Should().ContainSingle(
                logs => logs.Contains("Message has reached MaxDequeueCount of 3. Moving message to queue 'referrer-login-dev-local-poison'."),
                "message should have been moved to poison queue after three dequeues");

            var poisonMessages = await env.Queues.ReferrerLogin.Poison.FetchAll();
            var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

            poisonMessage.AsJson().Should().BeEquivalentTo(
                new JObject(
                    new JProperty("StartDateTime", "2022-05-17T00:00:00"),
                    new JProperty("EndDateTime", "2022-05-18T00:00:00")));

            var rows = await env.Postgres.Compute.ReferrerLogin.FetchAll();
            rows.Should().BeEmpty("the data should not have been inserted");

            functionLogs
                .Where(log => log.Contains(" Exception while executing function: ReferrerLogin_Compute_Queue. Npgsql: 23000: Simulating failure to insert with ReferrerId ReferrerFailure."))
                .Should().HaveCount(3, "function should should have attempted the insert three times");
        }
    }
}