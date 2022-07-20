using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.SectionView;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.MedicalRecord.SectionView;

[TestClass]
public class MedicalRecordSectionViewEventParserTests
{
    protected MedicalRecordSectionViewEventParser SystemUnderTest;

    [TestInitialize]
    public void Setup()
    {
        SystemUnderTest = new MedicalRecordSectionViewEventParser();
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

    [TestMethod]
    public void Parse_WhenNotSuccessfullyRetrieved_ReturnsNull()
    {
        var source = BuildEvent(details: "Patient record TEST RESULTS cannot be retrieved.");

        SystemUnderTest.Parse(source).Should().BeNull();
    }

    [TestMethod]
    public void Parse_WhenSectionIsNotCapitalised_ReturnsNull()
    {
        var source = BuildEvent(details: "Patient record lowercase section cannot be retrieved.");

        SystemUnderTest.Parse(source).Should().BeNull();
    }

    [TestMethod]
    public void Parse_ValidInputs_OtherFieldsSetCorrectly()
    {
        // Arrange
        var source = BuildEvent();

        // Act
        var parseResult = SystemUnderTest.Parse(source);

        // Assert
        parseResult.Should().BeOfType<MedicalRecordSectionViewMetric>();
        parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
        parseResult.SessionId.Should().Be("SessionId-test");
        parseResult.Supplier.Should().Be("Supplier-Test");
        parseResult.IsActingOnBehalfOfAnother.Should().Be(false);
        parseResult.Section.Should().Be("TEST RESULTS");
        parseResult.AuditId.Should().Be("AuditId-Test");
    }

    private static AuditRecord BuildEvent(
        string operation = "PatientRecord_Section_View_Response",
        string details = "Patient record TEST RESULTS successfully retrieved.")
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
