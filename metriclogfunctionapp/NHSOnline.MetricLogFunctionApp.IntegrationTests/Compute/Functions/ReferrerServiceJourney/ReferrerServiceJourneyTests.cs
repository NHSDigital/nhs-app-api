using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.ReferrerServiceJourney;

[TestClass]
public class ReferrerServiceJourneyTests
{
    [NhsAppTest]
    public async Task ReferrerServiceJourney_WithNoReferrer_ZeroRowsAddedToComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string auditId1 = "AuditId1";
        const string covidPassProvider = "the Department of Health and Social Care";
        const string otherProvider = "Other Provider";

        // Arrange
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1, false);
        await AddMetricHelper.AddAppointmentCancelMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 01, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddMedicalRecordViewMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 09, TimeSpan.Zero), sessionId1, true, true, "auditId1");
        await AddMetricHelper.AddNomPharmacyCreateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 03, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddNomPharmacyUpdateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 04, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationCreateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 05, TimeSpan.Zero), sessionId1, auditId1);
        await AddMetricHelper.AddOrganDonationGetMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 06, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationUpdateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 07, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationWithdrawMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 08, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddSilverIntegrationJumpOffMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 10, TimeSpan.Zero), sessionId1, "Provider1-ID", covidPassProvider, "JumpOffId1", "auditId1");
        await AddMetricHelper.AddSilverIntegrationJumpOffMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 11, TimeSpan.Zero), sessionId1, "Provider2-ID", otherProvider, "JumpOffId2", "auditId2");

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_AppointmentBookedServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1, false);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin,sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(1);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_AppointmentCancelledServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddAppointmentCancelMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(1);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.ReferrerId.Should().Be(referrerId);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_PrescriptionsServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(1);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_NomPharmacyCreateServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddNomPharmacyCreateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(1);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_NomPharmacyUpdateServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddNomPharmacyUpdateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(1);
            row.CovidPassJumpOffs.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_OdRegistrationsServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string auditId1 = "AuditId1";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddOrganDonationCreateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1, auditId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(1);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_OdWithdrawalsServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddOrganDonationWithdrawMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(1);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_OdUpdatesServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddOrganDonationUpdateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(1);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_OdLookupsServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddOrganDonationGetMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.OdLookups.Should().Be(1);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_RecordViewServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddMedicalRecordViewMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 09, TimeSpan.Zero), sessionId1, false, false, "auditId1");
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(1);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_RecordViewScrServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddMedicalRecordViewMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 09, TimeSpan.Zero), sessionId1, false, true, "auditId1");
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(1);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(1);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_RecordViewDcrServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddMedicalRecordViewMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 09, TimeSpan.Zero), sessionId1, true, false, "auditId1");
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(1);
            row.RecordViewsDcr.Should().Be(1);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_CovidPassJumpOffsServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";
        const string covidPassProvider = "the Department of Health and Social Care";

        // Arrange
        await AddMetricHelper.AddSilverIntegrationJumpOffMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 10, TimeSpan.Zero), sessionId1, "Provider1-ID", covidPassProvider, "JumpOffId1", "auditId1");
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(1);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_SilverIntegrationJumpOffsServiceJourneyWithReferrer_NewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "Other";
        const string referrerOrigin = "test";
        const string provider = "Other Provider";

        // Arrange
        await AddMetricHelper.AddSilverIntegrationJumpOffMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 11, TimeSpan.Zero), sessionId1, "Provider1-ID", provider, "JumpOffId1", "auditId1");
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(0);
            row.AppointmentsCancelled.Should().Be(0);
            row.CovidPassJumpOffs.Should().Be(0);
            row.NomPharmacyCreate.Should().Be(0);
            row.NomPharmacyUpdate.Should().Be(0);
            row.OdLookups.Should().Be(0);
            row.OdRegistrations.Should().Be(0);
            row.OdUpdates.Should().Be(0);
            row.OdWithdrawals.Should().Be(0);
            row.Prescriptions.Should().Be(0);
            row.RecordViews.Should().Be(0);
            row.RecordViewsDcr.Should().Be(0);
            row.RecordViewsScr.Should().Be(0);
            row.SilverIntegrationJumpOffs.Should().Be(1);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_MultipleServiceJourneyWithSingleReferrerDifferentSessionsSameDay_OnlyNewRecordsIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string sessionId2 = "SessionId2";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1, false);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 10, TimeSpan.Zero), sessionId2, false);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 10, TimeSpan.Zero), referrerId, referrerOrigin, sessionId2);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.First();
            row.ReferrerId.Should().Be(referrerId);
            row.Date.Should().Be(DateTime.Parse(startDateTime));
            row.AppointmentsBooked.Should().Be(2);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_DifferentServiceJourneysWithSameReferrerSameDay_OneNewRecordIsAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";
        const string covidPassProvider = "the Department of Health and Social Care";
        const string otherProvider = "Other Provider";
        const string auditId1 = "AuditId1";

        // Arrange
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1, false);
        await AddMetricHelper.AddAppointmentCancelMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 01, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddMedicalRecordViewMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 09, TimeSpan.Zero), sessionId1, true, true, "auditId1");
        await AddMetricHelper.AddNomPharmacyCreateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 03, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddNomPharmacyUpdateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 04, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationCreateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 05, TimeSpan.Zero), sessionId1, auditId1);
        await AddMetricHelper.AddOrganDonationGetMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 06, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationUpdateMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 07, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddOrganDonationWithdrawMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 08, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddSilverIntegrationJumpOffMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 10, TimeSpan.Zero), sessionId1, "Provider1-ID", covidPassProvider, "JumpOffId1", "auditId1");
        await AddMetricHelper.AddSilverIntegrationJumpOffMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 11, TimeSpan.Zero), sessionId1, "Provider2-ID", otherProvider, "JumpOffId2", "auditId2");
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.Date == DateTime.Parse(startDateTime));
            row.ReferrerId.Should().Be(referrerId);
            row.AppointmentsBooked.Should().Be(1);
            row.AppointmentsCancelled.Should().Be(1);
            row.CovidPassJumpOffs.Should().Be(1);
            row.NomPharmacyCreate.Should().Be(1);
            row.NomPharmacyUpdate.Should().Be(1);
            row.OdLookups.Should().Be(1);
            row.OdRegistrations.Should().Be(1);
            row.OdUpdates.Should().Be(1);
            row.OdWithdrawals.Should().Be(1);
            row.Prescriptions.Should().Be(1);
            row.RecordViews.Should().Be(1);
            row.RecordViewsDcr.Should().Be(1);
            row.RecordViewsScr.Should().Be(1);
            row.SilverIntegrationJumpOffs.Should().Be(1);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_MultipleServiceJourneyWithMultipleReferrerSameDay_TwoNewRecordsAreAddedInComputeTable(TestEnv env)
    {
        var startDateTime = "2022-05-17T00:00:00";
        var endDateTime = "2022-05-18T00:00:00";
        const string sessionId1 = "SessionId1";
        const string sessionId2 = "SessionId2";
        const string referrerId1 = "nhs-uk";
        const string referrerId2 = "other-referrer";
        const string referrerOrigin1 = "test1";
        const string referrerOrigin2 = "test2";

        // Arrange
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1, false);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId1, referrerOrigin1, sessionId1);

        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 10, TimeSpan.Zero), sessionId2, false);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 10, TimeSpan.Zero), referrerId2, referrerOrigin2, sessionId2);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(startDateTime, endDateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(2);

            rows.Should().AllSatisfy(x => x.AppointmentsBooked.Should().Be(1));
            rows.Should().AllSatisfy(x => x.Date.Should().Be(DateTime.Parse(startDateTime)));
            rows.Should().ContainSingle(x => x.ReferrerId == referrerId1);
            rows.Should().ContainSingle(x => x.ReferrerId == referrerId2);
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_MultipleServiceJourneyWithSingleReferrerDifferentDays_MultipleNewRecordsAreAddedInComputeTable(TestEnv env)
    {
        var day1DateTime = "2022-05-17T00:00:00";
        var day2DateTime = "2022-05-18T00:00:00";
        var day3DateTime = "2022-05-19T00:00:00";
        const string sessionId1 = "SessionId1";
        const string sessionId2 = "SessionId2";
        const string referrerId = "nhs-uk";
        const string referrerOrigin = "test";

        // Arrange
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), sessionId1, false);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 30, 00, TimeSpan.Zero), referrerId, referrerOrigin, sessionId1);

        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 18, 10, 30, 10, TimeSpan.Zero), sessionId2, false);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 18, 10, 30, 10, TimeSpan.Zero), referrerId, referrerOrigin, sessionId2);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(day1DateTime, day3DateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(2);

            rows.Should().AllSatisfy(x => x.AppointmentsBooked.Should().Be(1));
            rows.Should().AllSatisfy(x => x.ReferrerId.Should().Be(referrerId));
            rows.Should().ContainSingle(x => x.Date == DateTime.Parse(day1DateTime));
            rows.Should().ContainSingle(x => x.Date == DateTime.Parse(day2DateTime));
        }
    }

    [NhsAppTest]
    public async Task ReferrerServiceJourney_MultipleSessionsMultipleServiceJourneyWithMultipleReferrerDifferentDays_MultipleNewRecordsAreAddedInComputeTable(TestEnv env)
    {
        var day1DateTime = "2022-05-17T00:00:00";
        var day2DateTime = "2022-05-18T00:00:00";
        var day3DateTime = "2022-05-19T00:00:00";
        const string sessionId1 = "SessionId1";
        const string sessionId2 = "SessionId2";
        const string sessionId3 = "SessionId3";
        const string sessionId4 = "SessionId4";
        const string sessionId5 = "SessionId5";
        const string sessionId6 = "SessionId6";
        const string sessionId7 = "SessionId7";
        const string sessionId8 = "SessionId8";
        const string referrerId1 = "nhs-uk";
        const string referrerId2 = "other-referrer";
        const string referrerOrigin1 = "test1";
        const string referrerOrigin2 = "test2";

        // Arrange
        // day 1; referrer 1; session 1
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 26, 00, TimeSpan.Zero), sessionId1, false);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 17, 10, 26, 02, TimeSpan.Zero), sessionId1);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 26, 00, TimeSpan.Zero), referrerId1, referrerOrigin1, sessionId1);
        // day 1; referrer 2; session 1
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 27, 00, TimeSpan.Zero), sessionId2, false);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 17, 10, 27, 02, TimeSpan.Zero), sessionId2);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 27, 00, TimeSpan.Zero), referrerId2, referrerOrigin2, sessionId2);
        // day 1; referrer 1; session 2
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 28, 00, TimeSpan.Zero), sessionId3, false);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 17, 10, 28, 02, TimeSpan.Zero), sessionId3);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 28, 00, TimeSpan.Zero), referrerId1, referrerOrigin1, sessionId3);
        // day 1; referrer 2; session 2
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 17, 10, 29, 00, TimeSpan.Zero), sessionId4, false);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 17, 10, 29, 02, TimeSpan.Zero), sessionId4);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 17, 10, 29, 00, TimeSpan.Zero), referrerId2, referrerOrigin2, sessionId4);
        // day 2; referrer 1; session 1
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 18, 10, 30, 10, TimeSpan.Zero), sessionId5, false);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 18, 10, 30, 12, TimeSpan.Zero), sessionId5);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 18, 10, 30, 10, TimeSpan.Zero), referrerId1, referrerOrigin1, sessionId5);
        // day 2; referrer 2; session 1
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 18, 10, 31, 10, TimeSpan.Zero), sessionId6, false);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 18, 10, 31, 12, TimeSpan.Zero), sessionId6);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 18, 10, 31, 10, TimeSpan.Zero), referrerId2, referrerOrigin2, sessionId6);
        // day 2; referrer 1; session 2
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 18, 10, 32, 10, TimeSpan.Zero), sessionId7, false);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 18, 10, 32, 12, TimeSpan.Zero), sessionId7);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 18, 10, 32, 10, TimeSpan.Zero), referrerId1, referrerOrigin1, sessionId7);
        // day 2; referrer 2; session 2
        await AddMetricHelper.AddAppointmentBookMetric(env, new DateTimeOffset(2022, 05, 18, 10, 33, 10, TimeSpan.Zero), sessionId8, false);
        await AddMetricHelper.AddPrescriptionOrderMetric(env, new DateTimeOffset(2022, 05, 18, 10, 33, 12, TimeSpan.Zero), sessionId8);
        await AddMetricHelper.AddWebIntegrationReferralsMetric(env, new DateTimeOffset(2022, 05, 18, 10, 33, 10, TimeSpan.Zero), referrerId2, referrerOrigin2, sessionId8);

        // Act
        var response = await env.HttpEndpointCallers.PostReferrerServiceJourney(day1DateTime, day3DateTime);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.ReferrerServiceJourney.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.ReferrerServiceJourney.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(4);

            rows.Should().AllSatisfy(x => x.AppointmentsBooked.Should().Be(2));
            rows.Should().AllSatisfy(x => x.Prescriptions.Should().Be(2));
            rows.Should().ContainSingle(x => x.Date == DateTime.Parse(day1DateTime) && x.ReferrerId == referrerId1);
            rows.Should().ContainSingle(x => x.Date == DateTime.Parse(day1DateTime) && x.ReferrerId == referrerId2);
            rows.Should().ContainSingle(x => x.Date == DateTime.Parse(day2DateTime) && x.ReferrerId == referrerId1);
            rows.Should().ContainSingle(x => x.Date == DateTime.Parse(day2DateTime) && x.ReferrerId == referrerId2);
        }
    }
}
