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

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.DeviceInfo;

[TestClass]
public class DeviceInfoResilienceTests
{
    [NhsAppTest]
    public async Task DeviceInfo_InvalidRequestJson_FailsFast(TestEnv env)
    {
        var response = await env.HttpEndpointCallers.DeviceInfo.PostJson(new { StartDateTime = "2020-10-08T09:00:00", EndDateTime = "Blah" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();
        rows.Should().BeEmpty("the data should not have been inserted");

        var poisonMessages = await env.Queues.DeviceInfo.Poison.FetchAll();
        var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

        poisonMessage.AsJson().Should().BeEquivalentTo(
            new JObject(
                    new JProperty("StartDateTime", "2020-10-08T09:00:00"),
                    new JProperty("EndDateTime", "Blah"))
            );
    }

    [NhsAppTest]
    public async Task DeviceInfo_InsertDataFails_RetriesAndFails(TestEnv env)
    {
        var metricTimestamp = new DateTimeOffset(2022, 05, 17, 09, 00, 00, 00, TimeSpan.Zero);
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        var sessionId = "sessionId-test";
        var appVersion = "appVersionFailure";
        var deviceManufacturer = "deviceManufacturer-test";
        var deviceModel = "deviceModel-test";
        var deviceOS = "deviceOS-test";
        var deviceOSVersion = "deviceOS-test";
        var userAgent = "userAgent-test";

        await AddMetricHelper.AddDeviceMetric(env, metricTimestamp, sessionId, appVersion,
            deviceManufacturer, deviceModel, deviceOS, deviceOSVersion, userAgent);

        await env.Postgres.Compute.DeviceInfo.SetupTrigger(@"
    IF NEW.""AppVersion"" = 'appVersionFailure' THEN
        RAISE EXCEPTION 'Simulating failure to insert with AppVersion %', NEW.""AppVersion"" USING ERRCODE = 'integrity_constraint_violation';
    END IF;
");

        var response = await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var functionLogs = env.FunctionLogs();
        functionLogs.Should().ContainSingle(
            logs => logs.Contains("Message has reached MaxDequeueCount of 3. Moving message to queue 'device-info-dev-local-poison'."),
            "message should have been moved to poison queue after three dequeues");

        var poisonMessages = await env.Queues.DeviceInfo.Poison.FetchAll();
        var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

        poisonMessage.AsJson().Should().BeEquivalentTo(
            new JObject(
                new JProperty("StartDateTime", "2022-05-17T00:00:00"),
                new JProperty("EndDateTime", "2022-05-18T00:00:00")));

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();
        rows.Should().BeEmpty("the data should not have been inserted");

        functionLogs
            .Where(log => log.Contains(" Exception while executing function: DeviceInfo_Compute_Queue. Npgsql: 23000: Simulating failure to insert with AppVersion appVersionFailure."))
            .Should().HaveCount(3, "function should should have attempted the insert three times");
    }
}
