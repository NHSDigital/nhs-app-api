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

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.GPHealthRecord;

[TestClass]
public class GPHealthRecordResilienceTests
{
    [NhsAppTest]
    public async Task GPHealthRecord_InvalidRequestJson_FailsFast(TestEnv env)
    {
        var response = await env.HttpEndpointCallers.GPHealthRecord.PostJson(new { StartDateTime = "2022-06-10T09:00:00", EndDateTime = "InvalidDate" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.GPHealthRecord.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.GPRecordViews.FetchAll();
        rows.Should().BeEmpty("the data should not have been inserted");

        var poisonMessages = await env.Queues.GPHealthRecord.Poison.FetchAll();
        var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

        poisonMessage.AsJson().Should().BeEquivalentTo(
            new JObject(
                new JProperty("StartDateTime", "2022-06-10T09:00:00"),
                new JProperty("EndDateTime", "InvalidDate"))
        );
    }

    [NhsAppTest]
    public async Task GPHealthRecord_InsertDataFails_RetriesAndFails(TestEnv env)
    {
        var endDateTime = new DateTimeOffset(2022, 05, 26, 09, 00, 00, 00, TimeSpan.Zero);
        var endDateTimeString = "2022-05-27T00:00:00";
        var startDateTimeString = "2022-05-26T00:00:00";

        var supplier = "supplier1";
        var section = "DOCUMENTS";
        var loginId1 = "LoginId1";
        var odsCode = "Code";

        PatientViewsAGPHealthRecord(env, endDateTime, loginId1, section, supplier, odsCode);

        await env.Postgres.Compute.GPRecordViews.SetupTrigger(@"
            IF NEW.""OdsCode"" = 'Code' THEN
                RAISE EXCEPTION 'Simulating failure to insert with OdsCode %', NEW.""OdsCode"" USING ERRCODE = 'integrity_constraint_violation';
            END IF;
            ");

        // Act
        var response = await env.HttpEndpointCallers.PostGPHealthRecordViews(startDateTimeString, endDateTimeString);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.GPHealthRecord.WaitUntilEmpty();

        var functionLogs = env.FunctionLogs();
        functionLogs.Should().ContainSingle(
            logs => logs.Contains("Message has reached MaxDequeueCount of 3. Moving message to queue 'gp-health-record-dev-local-poison'."),
            "message should have been moved to poison queue after three dequeues");

        var poisonMessages = await env.Queues.GPHealthRecord.Poison.FetchAll();
        var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

        poisonMessage.AsJson().Should().BeEquivalentTo(
            new JObject(
                new JProperty("StartDateTime", startDateTimeString),
                new JProperty("EndDateTime", endDateTimeString)));

        var rows = await env.Postgres.Compute.GPRecordViews.FetchAll();
        rows.Should().BeEmpty("the data should not have been inserted");

        functionLogs
            .Where(log => log.Contains(" Exception while executing function: GPHealthRecord_Compute_Queue. Npgsql: 23000: Simulating failure to insert with OdsCode Code."))
            .Should().HaveCount(3, "function should should have attempted the insert three times");
    }

    private async void PatientViewsAGPHealthRecord(TestEnv env, DateTimeOffset viewTime, string loginId, string section,
        string supplier, string odsCode, bool hasSCR = true, bool hasDCR = false)
    {
        var sessionid = Guid.NewGuid().ToString();
        var auditId = sessionid;
        var p9ProofLevel = "P9";

        await AddMetricHelper.AddLoginMetric(env, loginId, p9ProofLevel, viewTime, sessionid, odsCode);
        await AddMetricHelper.AddMedicalRecordViewMetric(env, viewTime, sessionid, hasDCR, hasSCR, auditId);
        await AddMetricHelper.AddMedicalRecordSectionMetric(env, viewTime, sessionid, supplier, true, section, auditId);
    }
}
