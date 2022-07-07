using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Json;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Storage;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.Wayfinder;

[TestClass]
public class WayfinderResilienceTests
{
    [NhsAppTest]
    public async Task Wayfinder_InvalidRequestJson_FailsFast(TestEnv env)
    {
        var response = await env.HttpEndpointCallers.Wayfinder.PostJson(new { StartDateTime = "2022-06-10T09:00:00", EndDateTime = "InvalidDate" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.Wayfinder.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.Wayfinder.FetchAll();
        rows.Should().BeEmpty("the data should not have been inserted");

        var poisonMessages = await env.Queues.Wayfinder.Poison.FetchAll();
        var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

        poisonMessage.AsJson().Should().BeEquivalentTo(
            new JObject(
                    new JProperty("StartDateTime", "2022-06-10T09:00:00"),
                    new JProperty("EndDateTime", "InvalidDate"))
            );
    }

    [NhsAppTest]
    public async Task Wayfinder_InsertDataFails_RetriesAndFails(TestEnv env)
    {
        var startTime = "2022-05-17T00:00:00";
        var endTime = "2022-05-18T00:00:00";
        const string sessionId = "SessionFailure";
        const int totalReferrals = 1;
        const int totalUpcomingAppointments = 2;
        const string auditId = "AuditId1";

        await AddMetricHelper.AddSecondaryCareSummaryMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero),
            sessionId,
            totalReferrals,
            totalUpcomingAppointments,
            auditId);
        
        await env.Postgres.Compute.Wayfinder.SetupTrigger(@"
            IF NEW.""TotalReferrals"" = 1 THEN
                RAISE EXCEPTION 'Simulating failure to insert with TotalReferrals %', NEW.""TotalReferrals"" USING ERRCODE = 'integrity_constraint_violation';
            END IF;
        ");

        var response = await env.HttpEndpointCallers.PostWayfinder(startTime, endTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.Wayfinder.WaitUntilEmpty();

        var functionLogs = env.FunctionLogs();
        functionLogs.Should().ContainSingle(
            logs => logs.Contains("Message has reached MaxDequeueCount of 3. Moving message to queue 'wayfinder-dev-local-poison'."),
            "message should have been moved to poison queue after three dequeues");

        var poisonMessages = await env.Queues.Wayfinder.Poison.FetchAll();
        var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

        poisonMessage.AsJson().Should().BeEquivalentTo(
            new JObject(
                new JProperty("StartDateTime", "2022-05-17T00:00:00"),
                new JProperty("EndDateTime", "2022-05-18T00:00:00")));

        var rows = await env.Postgres.Compute.Wayfinder.FetchAll();
        rows.Should().BeEmpty("the data should not have been inserted");

        functionLogs
            .Where(log => log.Contains(" Exception while executing function: Wayfinder_Compute_Queue. Npgsql: 23000: Simulating failure to insert with TotalReferrals 1."))
            .Should().HaveCount(3, "function should should have attempted the insert three times");
    }
}
