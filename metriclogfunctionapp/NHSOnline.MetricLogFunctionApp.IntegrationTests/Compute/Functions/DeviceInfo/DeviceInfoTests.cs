using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.DeviceInfo;

[TestClass]
public class DeviceInfoTests
{
    [NhsAppTest]
    public async Task DeviceInfo_IsLoaded(TestEnv env)
    {
        // Arrange
        const string startDateTime = "2020-10-07T00:00:00";
        const string endDateTime = "2020-10-10T00:00:00";

        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 08, 09, 30, 00, TimeSpan.Zero), "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 09, 09, 30, 00, TimeSpan.Zero), "session2",
            "appVersion",
            "Manufacturer 2", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");

        // Act
        var response =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Should().HaveCount(2);
            var row1 = rows.Single(x => x.Date == new DateTime(2020, 10, 08));
            row1.DeviceManufacturer.Should().Be("Manufacturer 1");
            row1.Access.Should().NotBe(null);
            row1.AppVersion.Should().NotBe(null);
            row1.DeviceModel.Should().NotBe(null);
            row1.DeviceOS.Should().NotBe(null);
            row1.DeviceOSVersion.Should().NotBe(null);
        }
    }

    [NhsAppTest]
    public async Task DeviceInfo_StatsOutsideDateRange_IsNotLoaded(TestEnv env)
    {
        // Arrange
        const string startDateTime = "2020-10-07T00:00:00";
        const string endDateTime = "2020-10-09T00:00:00";

        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero), "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 08, 09, 30, 00, TimeSpan.Zero), "session2",
            "appVersion",
            "Manufacturer 2", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 10, 09, 30, 00, TimeSpan.Zero), "session3",
            "appVersion",
            "Manufacturer 3", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");

        // Act
        var response =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(2);
            var row1 = rows.Single(x => x.Date == new DateTime(2020, 10, 08));
            row1.DeviceManufacturer.Should().Be("Manufacturer 2");
            row1.Access.Should().NotBe(null);
            row1.AppVersion.Should().NotBe(null);
            row1.DeviceModel.Should().NotBe(null);
            row1.DeviceOS.Should().NotBe(null);
            row1.DeviceOSVersion.Should().NotBe(null);
        }
    }

    [NhsAppTest]
    public async Task DeviceInfo_OnInsert_RecordsSessionsCorrectly(TestEnv env)
    {
        // Arrange
        const string startDateTime = "2020-10-07T00:00:00";
        const string endDateTime = "2020-10-09T00:00:00";

        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero),
            "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero),
            "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero),
            "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");

        //Act
        var response =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);
            var row1 = rows.Single();
            row1.Sessions.Should().Be(3);
        }
    }

    [NhsAppTest]
    public async Task DeviceInfo_OnUpdate_NewRecordsAddToSessions(TestEnv env)
    {
        // Arrange
        const string startDateTime = "2020-10-07T00:00:00";
        const string endDateTime = "2020-10-09T00:00:00";

        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero),
            "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero),
            "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero),
            "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");

        //Act
        var response =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert before rerun
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);
            var row1 = rows.Single();
            row1.Sessions.Should().Be(3);
        }

        //Arrange - adding new metric record pre-rerun
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero),
            "session1",
            "appVersion",
            "Manufacturer 1", "deviceModel", "deviceOS", "deviceOSVersion", "userAgent");

        //Act - rerun
        var updateResponse =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        updateResponse.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var updatedRows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert after rerun
        using (new AssertionScope())
        {
            updatedRows.Count.Should().Be(1);
            var updatedRows1 = updatedRows.Single();
            updatedRows1.Sessions.Should().Be(4);
        }
    }

    [NhsAppTest]
    public async Task DeviceInfo_SetsDeviceModelCorrectly(TestEnv env)
    {
        // Arrange
        const string startDateTime = "2020-10-07T00:00:00";
        const string endDateTime = "2020-10-09T00:00:00";
        const string deviceIdentifier = "iPhone SE (1st generation)";
        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero), "session1",
            "appVersion",
            "Manufacturer 1", deviceIdentifier, "deviceOS", "deviceOSVersion", "userAgent");

        // Act
        var response =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);
            var row1 = rows.Single();
            row1.DeviceModel.Should().Be(deviceIdentifier);
        }
    }

    [NhsAppTest]
    public async Task DeviceInfo_SetsAppVersion_ToBrowserCorrectly(TestEnv env)
    {
        // Arrange
        const string startDateTime = "2020-10-07T00:00:00";
        const string endDateTime = "2020-10-09T00:00:00";

        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero), "session1",
            null,
            null, null, null, null, "userAgent");

        // Act
        var response =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);
            var row1 = rows.Single();
            row1.Access.Should().Be("Browser");
        }
    }

    [NhsAppTest]
    public async Task DeviceInfo_SetsAppVersion_ToMoblileBrowserCorrectly(TestEnv env)
    {
        // Arrange
        const string startDateTime = "2020-10-07T00:00:00";
        const string endDateTime = "2020-10-09T00:00:00";

        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero), "session1",
            null,
            null, null, null, null, "android");

        // Act
        var response =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);
            var row1 = rows.Single();
            row1.Access.Should().Be("Mobile browser");
        }
    }

    [NhsAppTest]
    public async Task DeviceInfo_SetsAppVersion_ToNativeCorrectly(TestEnv env)
    {
        // Arrange
        const string startDateTime = "2020-10-07T00:00:00";
        const string endDateTime = "2020-10-09T00:00:00";

        await AddMetricHelper.AddDeviceMetric(env, new DateTimeOffset(2020, 10, 07, 09, 30, 00, TimeSpan.Zero), "session1",
            "appVersion",
            "Manufacturer 1", "deviceIdentifier", "deviceOS", "deviceOSVersion", "userAgent");

        // Act
        var response =
            await env.HttpEndpointCallers.PostDeviceInfo(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.DeviceInfo.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.DeviceInfo.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);
            var row1 = rows.Single();
            row1.Access.Should().Be("Native");
        }
    }
}
