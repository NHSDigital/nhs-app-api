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

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.ReferrerServiceJourney;

[TestClass]
public class ReferrerServiceJourneyResilienceTests
{
    [NhsAppTest]
    public async Task ReferrerServiceJourney_InvalidRequestJson_FailsFast(TestEnv env)
    {
        var response = await env.HttpEndpointCallers.ReferrerServiceJourney.PostJson(new { StartDateTime = "2022-06-10T09:00:00", EndDateTime = "InvalidDate" });
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();
        rows.Should().BeEmpty("the data should not have been inserted");

        var poisonMessages = await env.Queues.ReferrerServiceJourney.Poison.FetchAll();
        var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

        poisonMessage.AsJson().Should().BeEquivalentTo(
            new JObject(
                    new JProperty("StartDateTime", "2022-06-10T09:00:00"),
                    new JProperty("EndDateTime", "InvalidDate"))
            );
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_InsertDataFails_RetriesAndFails(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string loginId1 = "LoginId1";
        const string referrerId = "ReferrerFailure";
        const string referrerOrigin = "test";
        const string covidPassProvider = "the Department of Health and Social Care";
        const string otherProvider = "Other Provider";
        const string auditId1 = "AuditId1";

        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1, false);
        await AddMetricHelper.AddAppointmentCancelMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 01, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddNomPharmacyCreateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 03, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddNomPharmacyUpdateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 04, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationCreateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 05, TimeSpan.Zero), sessionId1, auditId1);
        await AddMetricHelper.AddOrganDonationGetMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 06, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationUpdateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 07, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationWithdrawMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 08, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddMedicalRecordViewMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 09, TimeSpan.Zero), sessionId1, true, true, "auditId1");
        await AddMetricHelper.AddSilverIntegrationJumpOffMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 10, TimeSpan.Zero), sessionId1, "Provider1-ID", covidPassProvider, "JumpOffId1", "auditId1");
        await AddMetricHelper.AddSilverIntegrationJumpOffMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 11, TimeSpan.Zero), sessionId1, "Provider2-ID", otherProvider, "JumpOffId2", "auditId2");
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        await env.Postgres.Compute.ReferrerServiceJourney.SetupTrigger(@"
IF NEW.""ReferrerId"" = 'ReferrerFailure' THEN
    RAISE EXCEPTION 'Simulating failure to insert with ReferrerId %', NEW.""ReferrerId"" USING ERRCODE = 'integrity_constraint_violation';
END IF;
");

        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);

        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var functionLogs = env.FunctionLogs();
        functionLogs.Should().ContainSingle(
            logs => logs.Contains("Message has reached MaxDequeueCount of 3. Moving message to queue 'referrer-servicejourney-dev-local-poison'."),
            "message should have been moved to poison queue after three dequeues");

        var poisonMessages = await env.Queues.ReferrerServiceJourney.Poison.FetchAll();
        var poisonMessage = poisonMessages.Should().ContainSingle("the message should have been added to the poison queue once").Subject;

        poisonMessage.AsJson().Should().BeEquivalentTo(
            new JObject(
                new JProperty("StartDateTime", "2022-05-17T00:00:00"),
                new JProperty("EndDateTime", "2022-05-18T00:00:00")));

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();
        rows.Should().BeEmpty("the data should not have been inserted");

        functionLogs
            .Where(log => log.Contains(" Exception while executing function: ReferrerServiceJourney_Compute_Queue. Npgsql: 23000: Simulating failure to insert with ReferrerId ReferrerFailure."))
            .Should().HaveCount(3, "function should should have attempted the insert three times");
    }
}
