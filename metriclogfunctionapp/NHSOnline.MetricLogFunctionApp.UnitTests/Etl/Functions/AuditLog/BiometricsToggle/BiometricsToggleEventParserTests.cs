using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.BiometricsToggle;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.BiometricsToggle;

[TestClass]
public class BiometricsToggleEventParserTests
{
    protected BiometricsToggleEventParser SystemUnderTest;

    [TestInitialize]
    public void Setup()
    {
        SystemUnderTest = new BiometricsToggleEventParser();
    }

    [TestMethod]
    public void Parse_InvalidJson_ReturnsNull()
    {
        SystemUnderTest.Parse(null).Should().BeNull();
    }

    [DataTestMethod]
    [DataRow(null, DisplayName = "Null")]
    [DataRow("", DisplayName = "Blank")]
    [DataRow("InValidOperation", DisplayName = "Unrecognised")]
    public void Parse_InvalidOperation_ReturnsNull(string operation)
    {
        var source = BuildEvent(operation: operation);

        SystemUnderTest.Parse(source).Should().BeNull();
    }

    [DataTestMethod]
    [DataRow("True","On", DisplayName = "On")]
    [DataRow("False","Off", DisplayName = "Off")]
    public void Parse_ValidInputs_OtherFieldsSetCorrectly(string optIn, string result)
    {
        // Arrange
        var source = BuildEvent(biometricsToggle:optIn);

        // Act
        var parseResult = SystemUnderTest.Parse(source);

        // Assert
        parseResult.Should().BeOfType<BiometricsToggleMetric>();
        parseResult.SessionId.Should().Be("SessionId-test");
        parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
        parseResult.BiometricsToggle.Should().Be(result);
        parseResult.AuditId.Should().Be("AuditId-Test");
    }

    [TestMethod]
    public void Parse_MissingNotificationToggleShouldBeOff()
    {
        // Arrange
        var source = BuildEventNoBiometricsToggle();

        // Act
        var parseResult = SystemUnderTest.Parse(source);

        // Assert
        parseResult.Should().BeOfType<BiometricsToggleMetric>();
        parseResult.BiometricsToggle.Should().Be("Off");
    }


    private static AuditRecord BuildEvent(
            string operation = "BiometricsRegistration_Decision",
            string details = "Biometrics toggled.",
            string biometricsToggle = "true")
    {
        return new AuditRecord()
        {
            AuditId = "AuditId-Test",
            NhsLoginSubject = "NhsLoginSubject-Test",
            NhsNumber = "NhsNumber-Test",
            IsActingOnBehalfOfAnother = false,
            Supplier = "Supplier-Test",
            Operation = operation,
            Details = $"{details} optIn={biometricsToggle}",
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

    private static AuditRecord BuildEventNoBiometricsToggle(
        string operation = "BiometricsRegistration_Decision",
        string details = "Biometrics toggled.")
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
