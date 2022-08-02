using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env;
using NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Postgres.Events;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Etl.Functions.AuditLog.Device;

[TestClass]
public class DeviceMetricTests
{
    private string _sessionId1 = "SessionId1";
    private string _auditId1 = "AuditId1";
    private DateTime _timestamp = new(2021, 11, 01, 09, 00, 00, 1);
    private string _operation = "Login_Device";
    private string _details = "Device details returned: nhsapp-ios/AppVersion-Test nhsapp-manufacturer/DeviceManufacturer-Test " +
                                "nhsapp-model/DeviceModel-Test nhsapp-os/DeviceOSVersion-Test nhsapp-architecture/arm64";
    
    private AuditRecord _record;
    private DeviceRow _expectation;

    [TestInitialize]
    public void Setup()
    {
        _record = new AuditRecord
        {
            SessionId = _sessionId1,
            AuditId = _auditId1,
            Timestamp = _timestamp,
            Operation = _operation,
            Details = _details,
        };
        _expectation = new DeviceRow()
        { Timestamp = _timestamp, SessionId = _sessionId1, AuditId = _auditId1 };
    }

    [NhsAppTest]
    public async Task DeviceMetric_IsLoaded(TestEnv env)
    {
        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
        status.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.Device.FetchAll();
        var row = rows.Single(x => x.SessionId == _sessionId1);

        // Assert
        AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
    }

    [NhsAppTest]
    public async Task DeviceMetric_WhenEventIsRepeated_DuplicatesNotLoaded(TestEnv env)
    {
        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
        status.Should().Be(HttpStatusCode.OK);

        var duplicateStatus = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
        duplicateStatus.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.Device.FetchAll();

        var row = rows.Single(x => x.SessionId == _sessionId1);

        // Assert
        AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
    }

    [NhsAppTest]
    public async Task DeviceMetric_WhenRowExistsInDB_DuplicatesNotLoaded(TestEnv env)
    {
        // Stage
        await env.Postgres.Events.Device.Insert(new DeviceRow()
        {
            SessionId = _sessionId1,
            Timestamp = _timestamp,
            AuditId = _auditId1,
            UserAgent = _details.Replace("Device details returned:", string.Empty).Trim()
        });

        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, _record);
        status.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.Device.FetchAll();
        var row = rows.Single(x => x.SessionId == _sessionId1);

        // Assert
        AuditLogEtlTestHelper.AssertRowMatching(row, _expectation);
    }

    [NhsAppTest]
    public async Task
        DeviceMetric_AuditLogEventHubMessagesWithInvalidOperationField_AreNotLoaded(TestEnv env)
    {
        // Stage
        var invalidOperation = "invalid";
        var invalidRecord = new AuditRecord()
        {
            SessionId = _sessionId1,
            Timestamp = _timestamp,
            Operation = invalidOperation,
            Details = _details,
        };

        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, invalidRecord);
        status.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.Device.FetchAll();

        // Assert
        rows.Should().BeEmpty();
    }

    [NhsAppTest]
    public async Task DeviceMetric_AuditLogEventHubMessagesWithInvalidDetailsField_AreNotLoaded(
        TestEnv env)
    {
        // Stage
        var invalidDetails = "invalid";
        var invalidRecord = new AuditRecord()
        {
            SessionId = _sessionId1,
            Timestamp = _timestamp,
            Operation = _operation,
            Details = invalidDetails
        };

        // Act
        var status = await AuditLogEtlTestHelper.CreateAndPostAuditRecord(env, invalidRecord);
        status.Should().Be(HttpStatusCode.OK);

        var rows = await env.Postgres.Events.Device.FetchAll();

        // Assert
        rows.Should().BeEmpty();
    }
}
