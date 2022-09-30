using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using FluentAssertions.Execution;
using Microsoft.OData.Edm;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Http;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Compute;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Compute.Functions.GPHealthRecord;

[TestClass]
public class GPHealthRecordTests
{
    [NhsAppTest]
    public async Task GPHealthRecord_WhenAPatientViewsASectionOfTheirMedicalRecord_WillBeCounted(
        TestEnv env)
    {
        var endDateTime = new DateTimeOffset(2022, 05, 26, 09, 00, 00, 00, TimeSpan.Zero);
        var endDateTimeString = "2022-05-27T00:00:00";
        var startDateTimeString = "2022-05-26T00:00:00";

        var supplier = "supplier1";
        var section = "DOCUMENTS";
        var loginId1 = "LoginId1";
        var odsCode = "Code";

        PatientViewsAGPHealthRecord(env, endDateTime, loginId1, section, supplier, odsCode);

        // Act
        var response = await env.HttpEndpointCallers.PostGPHealthRecordViews(startDateTimeString, endDateTimeString);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.GPHealthRecord.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.GPRecordViews.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);
            var row = rows.Single(x => x.OdsCode == odsCode);
            row.Date.Should().Be(Date.Parse(startDateTimeString));
        }
    }

    [NhsAppTest]
    public async Task GPHealthRecord_WhenAMultiplePatientsViewDocumentsSection_WillBeGroupedByDocuments(
        TestEnv env)
    {
        var endDateTime = new DateTimeOffset(2022, 05, 26, 09, 00, 00, 00, TimeSpan.Zero);
        var endDateTimeString = "2022-05-27T00:00:00";
        var startDateTimeString = "2022-05-26T00:00:00";

        var supplier = "supplier1";
        var section = "DOCUMENTS";
        var odsCode = "Code";

        // Health Record View 1
        var loginId1 = "LoginId1";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId1, section, supplier, odsCode);

        // Health Record View 2
        var loginId2 = "LoginId2";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId2, section, supplier, odsCode);

        // Act
        var response = await env.HttpEndpointCallers.PostGPHealthRecordViews(startDateTimeString, endDateTimeString);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.GPHealthRecord.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.GPRecordViews.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row = rows.Single(x => x.OdsCode == odsCode);
            row.Date.Should().Be(Date.Parse(startDateTimeString));
            row.DocumentsSectionViewCount.Should().Be(2);
        }
    }

    [NhsAppTest]
    public async Task GPHealthRecord_WhenAMultiplePatientsViewDocumentsSection_WillBeGroupedByOdsCode(
        TestEnv env)
    {
        var endDateTime = new DateTimeOffset(2022, 05, 26, 09, 00, 00, 00, TimeSpan.Zero);
        var endDateTimeString = "2022-05-27T00:00:00";
        var startDateTimeString = "2022-05-26T00:00:00";

        var supplier = "supplier1";
        var section = "DOCUMENTS";

        // Health Record View for Ods Code 1
        var loginId1 = "LoginId1";
        var odsCode1 = "Code1";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId1, section, supplier, odsCode1);

        // Health Record View for Ods Code 2
        var loginId2 = "LoginId2";
        var odsCode2 = "Code2";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId2, section, supplier, odsCode2);

        var loginId3 = "LoginId3";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId3, section, supplier, odsCode2);

        // Act
        var response = await env.HttpEndpointCallers.PostGPHealthRecordViews(startDateTimeString, endDateTimeString);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.GPHealthRecord.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.GPRecordViews.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(2);

            var row1 = rows.Single(x => x.OdsCode == odsCode1);
            row1.Date.Should().Be(Date.Parse(startDateTimeString));
            row1.DocumentsSectionViewCount.Should().Be(1);
            row1.UniqueUsers.Should().Be(1);

            var row2 = rows.Single(x => x.OdsCode == odsCode2);
            row2.Date.Should().Be(Date.Parse(startDateTimeString));
            row2.DocumentsSectionViewCount.Should().Be(2);
            row2.UniqueUsers.Should().Be(2);
        }
    }

    [NhsAppTest]
    public async Task GPHealthRecord_WhenThereIsAlreadyAnEntryForAnOdsCode_EntryWillBeUpdatedWithLatestMetrics(
        TestEnv env)
    {
        var endDateTime = new DateTimeOffset(2022, 05, 26, 09, 00, 00, 00, TimeSpan.Zero);
        var endDateTimeString = "2022-05-27T00:00:00";
        var startDateTimeString = "2022-05-26T00:00:00";

        var allergySection = "ALLERGIES AND ADVERSE REACTIONS";
        var documentsSection = "DOCUMENTS";
        var odsCode = "Code";
        var supplier1 = "supplier1";

        await env.Postgres.Compute.GPRecordViews.Insert(new GPRecordViewsRows
        {
            OdsCode = odsCode,
            Date = endDateTime.Date,
            Supplier = supplier1,
            HealthRecordViews = 1,
            UniqueUsers = 1,
            ViewsWithSummaryRecordAccess = 1,
            ViewsWithDetailedRecordAccess = 0,
            IsActingOnBehalfOfAnother = 0,
            DocumentsSectionViewCount = 1
        });

        // Health Record View for Allergies Section
        var loginId1 = "LoginId1";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId1, allergySection, supplier1, odsCode);

        // Health Record View for Documents Section
        var loginId2 = "LoginId2";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId2, documentsSection, supplier1, odsCode);

        // Health Record View for Documents Section
        var loginId3 = "LoginId3";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId3, documentsSection, supplier1, odsCode);

        // Act
        var response = await env.HttpEndpointCallers.PostGPHealthRecordViews(startDateTimeString, endDateTimeString);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.GPHealthRecord.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.GPRecordViews.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row1 = rows.Single(x => x.OdsCode == odsCode);
            row1.Date.Should().Be(Date.Parse(startDateTimeString));
            row1.OdsCode.Should().Be(odsCode);
            row1.DocumentsSectionViewCount.Should().Be(2);
            row1.AllergiesAdverseReactionsSectionViewCount.Should().Be(1);
            row1.UniqueUsers.Should().Be(3);
        }
    }

    [NhsAppTest]
    public async Task GPHealthRecord_WhenAMultiplePatientsViewDocumentsSection_WillCountTheCorrectNumberOfUserWithSCRAccess(
        TestEnv env)
    {
        var endDateTime = new DateTimeOffset(2022, 05, 26, 09, 00, 00, 00, TimeSpan.Zero);
        var endDateTimeString = "2022-05-27T00:00:00";
        var startDateTimeString = "2022-05-26T00:00:00";

        var odsCode = "Code";
        var section = "DOCUMENTS";
        var supplier = "supplier";

        // Health Record View for user with SCR
        var loginId1 = "LoginId1";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId1, section, supplier, odsCode, hasSCR:true, hasDCR:false);

        var loginId2 = "LoginId2";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId2, section, supplier, odsCode, hasSCR:true, hasDCR:false);

        var loginId3 = "LoginId3";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId3, section, supplier, odsCode, hasSCR:true, hasDCR:false);

        // Act
        var response = await env.HttpEndpointCallers.PostGPHealthRecordViews(startDateTimeString, endDateTimeString);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.GPHealthRecord.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.GPRecordViews.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row1 = rows.Single(x => x.OdsCode == odsCode);
            row1.Date.Should().Be(Date.Parse(startDateTimeString));
            row1.UniqueUsers.Should().Be(3);
            row1.ViewsWithSummaryRecordAccess.Should().Be(3);
            row1.ViewsWithDetailedRecordAccess.Should().Be(0);
        }
    }

    [NhsAppTest]
    public async Task GPHealthRecord_WhenAMultiplePatientsViewDocumentsSection_WillCountTheCorrectNumberOfUsersWithSCRAndDCRAccess(
        TestEnv env)
    {
        var endDateTime = new DateTimeOffset(2022, 05, 26, 09, 00, 00, 00, TimeSpan.Zero);
        var endDateTimeString = "2022-05-27T00:00:00";
        var startDateTimeString = "2022-05-26T00:00:00";

        var odsCode = "Code";
        var section = "DOCUMENTS";
        var supplier = "supplier";

        // Health Record View for user with DCR
        var loginId1 = "LoginId1";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId1, section, supplier, odsCode, hasSCR:false, hasDCR:true);

        // Health Record View for user with SCR
        var loginId2 = "LoginId2";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId2, section, supplier, odsCode, hasSCR:true, hasDCR:false);

        var loginId3 = "LoginId3";
        PatientViewsAGPHealthRecord(env, endDateTime, loginId3, section, supplier, odsCode, hasSCR:true, hasDCR:false);

        // Act
        var response = await env.HttpEndpointCallers.PostGPHealthRecordViews(startDateTimeString, endDateTimeString);
        response.StatusCode.Should().Be(HttpStatusCode.Created);
        await env.Queues.GPHealthRecord.WaitUntilEmpty();

        var rows = await env.Postgres.Compute.GPRecordViews.FetchAll();

        // Assert
        using (new AssertionScope())
        {
            rows.Count.Should().Be(1);

            var row1 = rows.Single(x => x.OdsCode == odsCode);
            row1.Date.Should().Be(Date.Parse(startDateTimeString));
            row1.UniqueUsers.Should().Be(3);
            row1.ViewsWithDetailedRecordAccess.Should().Be(1);
            row1.ViewsWithSummaryRecordAccess.Should().Be(2);
        }
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
