using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier;

[TestClass]
public class LastLoginPatientIdentifierTests
{
    private string _sessionId = "SessionId1";
    private string _auditId = "AuditId1";
    private string _nhsNumber = "NhsNumber-Test";
    private string _loginId = "NhsLoginSubject-Test";
    private DateTime _timestamp = new(2021, 11, 01, 09, 00, 00, 1);
    private string _operation = "CitizenId_Session_Create_Request";
    private string _details = "Create Citizen Id Session";

    private AuditRecord _record;
    private LastLoginPatientIdentifierRow _expectation;

    [TestInitialize]
    public void Setup()
    {
        _record = new AuditRecord
        {
            SessionId = _sessionId,
            AuditId = _auditId,
            Timestamp = _timestamp,
            Operation = _operation,
            Details = _details,
            NhsNumber = "NhsNumber-Test",
            NhsLoginSubject = _loginId
        };
        _expectation = new LastLoginPatientIdentifierRow
        { Timestamp = _timestamp, NhsNumber = _nhsNumber, LoginId = _loginId, AuditId = _auditId };
    }

    [NhsAppTest]
    public async Task LastLoginPatientIdentifier_IsLoaded(TestEnv env)
    {
        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
        status.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.LastLoginPatientIdentifier.FetchAll();
        var row = rows.Single(x => x.LoginId == _loginId);

        // Assert
        AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
    }

    [NhsAppTest]
    public async Task LastLoginPatientIdentifier_WhenEventIsRepeated_DuplicatesNotLoaded(TestEnv env)
    {
        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
        status.Should().Be(HttpStatusCode.OK);

        var duplicateStatus = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
        duplicateStatus.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.LastLoginPatientIdentifier.FetchAll();
        var row = rows.Single(x => x.LoginId == _loginId);

        // Assert
        AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
    }

    [NhsAppTest]
    public async Task LastLoginPatientIdentifier_WhenRowExistsInDB_DuplicatesNotLoaded(TestEnv env)
    {
        // Stage
        await env.Postgres.Events.LastLoginPatientIdentifier.Insert(new LastLoginPatientIdentifierRow
        {
            Timestamp = _timestamp,
            NhsNumber = _nhsNumber,
            LoginId = _loginId,
            AuditId = _auditId
        }
        );

        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
        status.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.LastLoginPatientIdentifier.FetchAll();
        var row = rows.Single(x => x.LoginId == _loginId);

        // Assert
        AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
    }

    [NhsAppTest]
    public async Task LastLoginPatientIdentifier_AuditLogEventHubMessagesWithInvalidOperationField_AreNotLoaded(TestEnv env)
    {
        // Stage
        const string invalidOperation = "invalid";
        var invalidRecord = new AuditRecord
        {
            SessionId = _sessionId,
            Timestamp = _timestamp,
            Operation = invalidOperation,
            Details = _details,
            AuditId = _auditId,
            NhsNumber = _nhsNumber,
            NhsLoginSubject = _loginId,
        };

        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, invalidRecord);
        status.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.LastLoginPatientIdentifier.FetchAll();

        // Assert
        rows.Should().BeEmpty();
    }

    [NhsAppTest]
    public async Task LastLoginPatientIdentifier_AuditLogEventHubMessagesWithInvalidDetailsField_AreNotLoaded(TestEnv env)
    {
        // Stage
        const string invalidDetails = "invalid";
        var invalidRecord = new AuditRecord()
        {
            SessionId = _sessionId,
            Timestamp = _timestamp,
            Operation = _operation,
            Details = invalidDetails,
            AuditId = _auditId,
            NhsNumber = _nhsNumber,
            NhsLoginSubject = _loginId,
        };

        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, invalidRecord);
        status.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.LastLoginPatientIdentifier.FetchAll();

        // Assert
        rows.Should().BeEmpty();
    }
}
