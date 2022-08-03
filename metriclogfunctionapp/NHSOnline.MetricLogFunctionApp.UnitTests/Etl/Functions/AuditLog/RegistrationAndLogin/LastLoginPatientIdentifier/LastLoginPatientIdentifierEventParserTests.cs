using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier;
using LastLoginPatientIdentifierMetric = NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier.LastLoginPatientIdentifier;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.RegistrationAndLogin.LastLoginPatientIdentifier;

[TestClass]
public class LastLoginPatientIdentifierEventParserTests
{
    private LastLoginPatientIdentifierEventParser _systemUnderTest;

    [TestInitialize]
    public void Setup() => _systemUnderTest = new LastLoginPatientIdentifierEventParser();

    [TestMethod]
    public void Parse_InvalidJson_ReturnsNull() => _systemUnderTest.Parse(null).Should().BeNull();

    [DataTestMethod]
    [DataRow(null, DisplayName = "Null")]
    [DataRow("", DisplayName = "Blank")]
    [DataRow("InValidOperation", DisplayName = "Unrecognised")]
    public void Parse_InvalidOperation_ReturnsNull(string operation)
    {
        var source = BuildEvent(operation: operation);
        _systemUnderTest.Parse(source).Should().BeNull();
    }

    [TestMethod]
    public void Parse_ValidInputs_OtherFieldsSetCorrectly()
    {
        // Arrange
        var source = BuildEvent();

        // Act
        var parseResult = _systemUnderTest.Parse(source);

        // Assert
        parseResult.Should().BeOfType<LastLoginPatientIdentifierMetric>();
        parseResult.LoginId.Should().Be("NhsLoginSubject-Test");
        parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
        parseResult.NhsNumber.Should().Be("NhsNumber-Test");
        parseResult.AuditId.Should().Be("AuditId-Test");
    }

    [TestMethod]
    public void Parse_IrrelevantDetails_ReturnsNull()
    {
        // Arrange
        var source = BuildEvent(details: "A not relevant detail");

        // Act & Assert
        _systemUnderTest.Parse(source).Should().BeNull();
    }

    private static AuditRecord BuildEvent(
        string operation = "CitizenId_Session_Create_Request",
        string details = "Create Citizen Id Session")
    {
        return new AuditRecord()
        {
            AuditId = "AuditId-Test",
            NhsLoginSubject = "NhsLoginSubject-Test",
            NhsNumber = "NhsNumber-Test",
            IsActingOnBehalfOfAnother = false,
            Supplier = "Supplier-Test",
            Operation = operation,
            Details = details,
            ApiVersion = "Api-Test",
            WebVersion = "Web-Test",
            NativeVersion = "NativeVersion-Test",
            Environment = "localtest",
            SessionId = "SessionId-test",
            Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 123),
            ProofLevel = "P5",
            ODS = "ods1",
            Referrer = "ref1"
        };
    }
}
