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

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.FirstLogins
{
    [TestClass]
    public class FirstLoginsResilienceTests
    {
        [NhsAppTest]
        public async Task FirstLogins_InvalidRequestJson_FailsFast(TestEnv env)
        {
           var response = await env.HttpEndpointCallers.FirstLogins.PostJson(new { LoginId = "LoginId", StartDateTime = "2020-10-08T09:00:00", EndDateTime = "Blah"});
            response.StatusCode.Should().Be(HttpStatusCode.Created);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();
            rows.Should().BeEmpty("the data should not have been inserted");

            var poisonMessages = await env.Queues.FirstLogins.Poison.FetchAll();
            var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

            poisonMessage.AsJson().Should().BeEquivalentTo(
                new JObject(
                        new JProperty("LoginId", "LoginId"),
                        new JProperty("StartDateTime", "2020-10-08T09:00:00"),
                        new JProperty("EndDateTime", "Blah"))
                );
        }

        [NhsAppTest]
        public async Task FirstLogins_InsertDataFails_RetriesAndFails(TestEnv env)
        {
            var events = new[]
            {
                FirstLoginMetrics.BuildEvent("AuditId1", "SessionId", new DateTime(2020, 10, 08, 09, 30, 00, DateTimeKind.Utc), "Login_Success",
                    "Successful Login with", "P5", "AB123", "ref1","LoginIdFailure")
            };

            await env.Postgres.Compute.FirstLogins.SetupTrigger(@"
    IF NEW.""LoginId"" = 'LoginIdFailure' THEN
        RAISE EXCEPTION 'Simulating failure to insert with LoginId %', NEW.""LoginId"" USING ERRCODE = 'integrity_constraint_violation';
    END IF;
");

            var response = await env.HttpEndpointCallers.PostAuditLogConsumer(events);
            response.StatusCode.Should().Be(HttpStatusCode.OK);

            await env.Queues.FirstLogins.WaitUntilEmpty();

            var functionLogs = env.FunctionLogs();
            functionLogs.Should().ContainSingle(
                logs => logs.Contains("Message has reached MaxDequeueCount of 3. Moving message to queue 'first-logins-metric-dev-local-poison'."),
                "message should have been moved to poison queue after three dequeues");

            var poisonMessages = await env.Queues.FirstLogins.Poison.FetchAll();
            var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

            poisonMessage.AsJson().Should().BeEquivalentTo(
                new JObject(
                    new JProperty("LoginId", "LoginIdFailure"),
                    new JProperty("StartDateTime", "2020-10-08T09:29:59"),
                    new JProperty("EndDateTime", "2020-10-08T09:30:01")));

            var rows = await env.Postgres.Compute.FirstLogins.FetchAll();
            rows.Should().BeEmpty("the data should not have been inserted");

            functionLogs
                .Where(log => log.Contains(" Exception while executing function: FirstLogins_Compute_Queue. Npgsql: 23000: Simulating failure to insert with LoginId LoginIdFailure."))
                .Should().HaveCount(3, "function should should have attempted the insert three times");
        }
    }
}