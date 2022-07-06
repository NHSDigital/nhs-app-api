using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.OrganDonationRegistration.Update;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.OrganDonationRegistration.Update
{
    [TestClass]
    public class OrganDonationRegistrationUpdateEventParserTests
    {
        protected OrganDonationRegistrationUpdateEventParser SystemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SystemUnderTest = new OrganDonationRegistrationUpdateEventParser();
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
            parseResult.Should().BeOfType<OrganDonationRegistrationUpdateMetric>();
            parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
            parseResult.SessionId.Should().Be("SessionId-test");
            parseResult.AuditId.Should().Be("AuditId-Test");
        }

        private static AuditRecord BuildEvent(
            string operation = "OrganDonation_Update_Response",
            string details = "The organ donation decision has been successfully updated")
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
