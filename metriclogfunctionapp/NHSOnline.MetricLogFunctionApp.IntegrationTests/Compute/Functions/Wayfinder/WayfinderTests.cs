using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.Wayfinder;

[TestClass]
public class WayfinderTests
{
    [NhsAppTest]
    public async Task Wayfinder_SingleSecondaryCareSummaryMetricRecord_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startTime = "2022-05-17T00:00:00";
        var endTime = "2022-05-18T00:00:00";
        const string loginId = "LoginId";
        const string sessionId = "SessionId";
        const string proofLevel = "P9";
        const int totalReferrals = 1;
        const int totalUpcomingAppointments = 2;
        const string auditId = "auditId";

        // Arrange
        await AddMetricHelper.AddLoginMetric(env,
            loginId,
            proofLevel,
            new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero));
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero),
            sessionId,
            totalReferrals,
            totalUpcomingAppointments,
            auditId);

        // Act
        var response = await env.HttpEndpointCallers.PostWayfinder(startTime, endTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.Wayfinder.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.Wayfinder.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startTime));
            row.TotalSessions.Should().Be(1);
            row.TotalViews.Should().Be(1);
            row.Users.Should().Be(1);
            row.TotalReferrals.Should().Be(totalReferrals);
            row.TotalUpcomingAppointments.Should().Be(totalUpcomingAppointments);
        }
    }

    [NhsAppTest]
    public async Task Wayfinder_MultipleSecondaryCareSummaryMetricRecordsSameUserSession_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startTime = "2022-05-17T00:00:00";
        var endTime = "2022-05-18T00:00:00";
        const string loginId = "LoginId";
        const string sessionId = "SessionId";
        const string proofLevel = "P9";
        const int totalReferrals1 = 1;
        const int totalUpcomingAppointments1 = 2;
        const int totalReferrals2 = 10;
        const int totalUpcomingAppointments2 = 20;
        const string auditId1 = "auditId1";
        const string auditId2 = "auditId2";

        // Arrange
        await AddMetricHelper.AddLoginMetric(env,
            loginId,
            proofLevel,
            new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero));
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero),
            sessionId,
            totalReferrals1,
            totalUpcomingAppointments1,
            auditId1);
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            new DateTimeOffset(2022, 05, 17, 10, 30, 01, TimeSpan.Zero),
            sessionId,
            totalReferrals2,
            totalUpcomingAppointments2,
            auditId2);

        // Act
        var response = await env.HttpEndpointCallers.PostWayfinder(startTime, endTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.Wayfinder.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.Wayfinder.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startTime));
            row.TotalSessions.Should().Be(1);
            row.TotalViews.Should().Be(2);
            row.Users.Should().Be(1);
            row.TotalReferrals.Should().Be(totalReferrals1 + totalReferrals2);
            row.TotalUpcomingAppointments.Should().Be(totalUpcomingAppointments1 + totalUpcomingAppointments2);
        }
    }

    [NhsAppTest]
    public async Task Wayfinder_MultipleSecondaryCareSummaryMetricRecordsSameUserDifferentSessions_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startTime = "2022-05-17T00:00:00";
        var endTime = "2022-05-18T00:00:00";
        const string loginId = "LoginId";
        const string sessionId1 = "SessionId1";
        const string sessionId2 = "SessionId2";
        const string proofLevel = "P9";
        const int totalReferrals1 = 1;
        const int totalUpcomingAppointments1 = 2;
        const int totalReferrals2 = 10;
        const int totalUpcomingAppointments2 = 20;
        const string auditId1 = "auditId1";
        const string auditId2 = "auditId2";

        // Arrange
        await AddMetricHelper.AddLoginMetric(env,
            loginId,
            proofLevel,
            new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero),
            sessionId1);
        await AddMetricHelper.AddLoginMetric(env,
            loginId,
            proofLevel,
            new DateTimeOffset(2022, 05, 17, 10, 30, 01, TimeSpan.Zero),
            sessionId2);
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero),
            sessionId1,
            totalReferrals1,
            totalUpcomingAppointments1,
            auditId1);
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            new DateTimeOffset(2022, 05, 17, 10, 30, 03, TimeSpan.Zero),
            sessionId2,
            totalReferrals2,
            totalUpcomingAppointments2,
            auditId2);

        // Act
        var response = await env.HttpEndpointCallers.PostWayfinder(startTime, endTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.Wayfinder.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.Wayfinder.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startTime));
            row.TotalSessions.Should().Be(2);
            row.TotalViews.Should().Be(2);
            row.Users.Should().Be(1);
            row.TotalReferrals.Should().Be(totalReferrals1 + totalReferrals2);
            row.TotalUpcomingAppointments.Should().Be(totalUpcomingAppointments1 + totalUpcomingAppointments2);
        }
    }

    [NhsAppTest]
    public async Task Wayfinder_MultipleSecondaryCareSummaryMetricRecordsDifferentUsers_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startTime = "2022-05-17T00:00:00";
        var endTime = "2022-05-18T00:00:00";
        const string loginId1 = "LoginId1";
        const string loginId2 = "LoginId2";
        const string sessionId1 = "SessionId1";
        const string sessionId2 = "SessionId2";
        const string proofLevel = "P9";
        const int totalReferrals1 = 1;
        const int totalUpcomingAppointments1 = 2;
        const int totalReferrals2 = 10;
        const int totalUpcomingAppointments2 = 20;
        const string auditId1 = "auditId1";
        const string auditId2 = "auditId2";

        // Arrange
        await AddMetricHelper.AddLoginMetric(env,
            loginId1,
            proofLevel,
            new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero),
            sessionId1);
        await AddMetricHelper.AddLoginMetric(env,
            loginId2,
            proofLevel,
            new DateTimeOffset(2022, 05, 17, 10, 30, 01, TimeSpan.Zero),
            sessionId2);
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero),
            sessionId1,
            totalReferrals1,
            totalUpcomingAppointments1,
            auditId1);
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            new DateTimeOffset(2022, 05, 17, 10, 30, 03, TimeSpan.Zero),
            sessionId2,
            totalReferrals2,
            totalUpcomingAppointments2,
            auditId2);

        // Act
        var response = await env.HttpEndpointCallers.PostWayfinder(startTime, endTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.Wayfinder.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.Wayfinder.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startTime));
            row.TotalSessions.Should().Be(2);
            row.TotalViews.Should().Be(2);
            row.Users.Should().Be(2);
            row.TotalReferrals.Should().Be(totalReferrals1 + totalReferrals2);
            row.TotalUpcomingAppointments.Should().Be(totalUpcomingAppointments1 + totalUpcomingAppointments2);
        }
    }

    [NhsAppTest]
    public async Task Wayfinder_When_UniqueConstraintIsViolated_Expect_RowToBeInsertedReplacesExistingRow(TestEnv env)
    {
        // Arrange
        var endDate = new DateTimeOffset(2022, 05, 27, 00, 00, 00, 00, TimeSpan.Zero);
        var endDateString = "2022-05-27T00:00:00";
        var startDateString = "2022-05-26T00:00:00";
        const string loginId = "LoginId";
        const string sessionId1 = "SessionId1";
        const string sessionId2 = "SessionId2";
        const string proofLevel = "P9";
        const int totalReferrals1 = 1;
        const int totalUpcomingAppointments1 = 2;
        const int totalReferrals2 = 10;
        const int totalUpcomingAppointments2 = 20;
        const string auditId1 = "auditId1";
        const string auditId2 = "auditId2";

        // Arrange
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            endDate.AddHours(-1),
            sessionId1,
            totalReferrals1,
            totalUpcomingAppointments1,
            auditId1);

        await AddMetricHelper.AddLoginMetric(env,
            loginId,
            proofLevel,
            endDate.AddHours(-1),
            sessionId1);

        var response = await env.HttpEndpointCallers.PostWayfinder(startDateString, endDateString);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.Wayfinder.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.Wayfinder.FetchAll();

        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);
            var row = rows.Single(x => x.Date == DateTime.Parse(startDateString));

            row.TotalSessions.Should().Be(1);
            row.TotalViews.Should().Be(1);
            row.Users.Should().Be(1);
            row.TotalReferrals.Should().Be(totalReferrals1);
            row.TotalUpcomingAppointments.Should().Be(totalUpcomingAppointments1);
        }
        await AddMetricHelper.AddSecondaryCareSummaryMetric(env,
            endDate.AddHours(-1),
            sessionId2,
            totalReferrals2,
            totalUpcomingAppointments2,
            auditId2);

        await AddMetricHelper.AddLoginMetric(env,
            loginId,
            proofLevel,
            endDate.AddHours(-1),
            sessionId2);

        //Act
        var secondResponse = await env.HttpEndpointCallers.PostWayfinder(startDateString, endDateString);
        secondResponse.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.Wayfinder.WaitUntilEmpty();

        var newRows = await env.Postgres.Compute.Wayfinder.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            newRows.Count.Should().Be(1);

            var row = newRows.Single(x => x.Date == DateTime.Parse(startDateString));
            row.TotalSessions.Should().Be(2);
            row.TotalViews.Should().Be(2);
            row.Users.Should().Be(1);
            row.TotalReferrals.Should().Be(totalReferrals1 + totalReferrals2);
            row.TotalUpcomingAppointments.Should().Be(totalUpcomingAppointments1 + totalUpcomingAppointments2);
        }
    }
}
