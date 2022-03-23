using System;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.RegistrationAndLogin.Consent
{
    [TestClass]
    public class ConsentEventParserTests
    {
        protected ConsentEventParser SystemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SystemUnderTest = new ConsentEventParser();
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
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Blank")]
        [DataRow("InValidDetails", DisplayName = "Unrecognised")]
        public void Parse_InvalidDetails_ReturnsNull(string details)
        {
            var source = BuildEvent(details: details);

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
            parseResult.Should().BeOfType<ConsentMetric>();
            parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
            parseResult.SessionId.Should().Be("SessionId-test");
            parseResult.AuditId.Should().Be("AuditId-Test");
        }

        private static AuditRecord BuildEvent(
            string operation = "TermsAndConditions_RecordConsent_Response",
            string details = "Initial Consent Successfully recorded")
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

