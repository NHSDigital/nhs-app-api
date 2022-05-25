using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.MedicalRecord.MedicalRecordView
{
    [TestClass]
    public class MedicalRecordViewEventParserTests
    {
        protected MedicalRecordViewEventParser SystemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SystemUnderTest = new MedicalRecordViewEventParser();
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
        public void Parse_ValidInputs_OtherFieldsSetCorrectly()
        {
            // Arrange
            var source = BuildEvent();

            // Act
            var parseResult = SystemUnderTest.Parse(source);

            // Assert
            parseResult.Should().BeOfType<MedicalRecordViewMetric>();
            parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
            parseResult.SessionId.Should().Be("SessionId-test");
            parseResult.HasSummaryRecordAccess.Should().Be(true);
            parseResult.HasDetailedRecordAccess.Should().Be(true);
            parseResult.AuditId.Should().Be("AuditId-Test");
        }

        [TestMethod]
        public void Parse_MissingReferralsAndAppointments_ReferralsAndAppointmentsShouldBeZero()
        {
            // Arrange
            var source = BuildEventNoRecordAccessPresent();

            // Act
            var parseResult = SystemUnderTest.Parse(source);

            // Assert
            parseResult.Should().BeOfType<MedicalRecordViewMetric>();
            parseResult.HasSummaryRecordAccess.Should().Be(false);
            parseResult.HasDetailedRecordAccess.Should().Be(false);
        }

        private static AuditRecord BuildEvent(
            string operation = "PatientRecord_View_Response",
            string details = "Patient record successfully retrieved",
            string hasSummaryRecordAccess = "True",
            string hasDetailedRecordAccess = "True")
        {
            return new AuditRecord()
            {
                AuditId = "AuditId-Test",
                NhsLoginSubject = "NhsLoginSubject-Test",
                NhsNumber = "NhsNumber-Test",
                IsActingOnBehalfOfAnother = false,
                Supplier = "Supplier-Test",
                Operation = operation,
                Details = $"{details}. hasSummaryRecordAccess={hasSummaryRecordAccess}, hasDetailedRecordAccess={hasDetailedRecordAccess}",
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

        private static AuditRecord BuildEventNoRecordAccessPresent(
            string operation = "PatientRecord_View_Response",
            string details = "Patient record successfully retrieved")
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
}