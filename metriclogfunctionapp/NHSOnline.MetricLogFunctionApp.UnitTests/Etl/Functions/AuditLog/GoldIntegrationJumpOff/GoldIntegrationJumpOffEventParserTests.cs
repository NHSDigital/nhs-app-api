using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.GoldIntegrationJumpOff;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.GoldIntegrationJumpOff
{
    [TestClass]
    public class GoldIntegrationJumpOffEventParserTests
    {
        protected GoldIntegrationJumpOffEventParser SystemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SystemUnderTest = new GoldIntegrationJumpOffEventParser();
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
            parseResult.Should().BeOfType<GoldIntegrationJumpOffMetric>();
            parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 01, 09, 00, 00, 123));
            parseResult.SessionId.Should().Be("SessionId-test");
            parseResult.ProviderId.Should().Be("eConsult");
            parseResult.ProviderName.Should().Be("eConsult");
            parseResult.JumpOffId.Should().Be("onlineConsultation");
            parseResult.AuditId.Should().Be("AuditId-Test");
        }

        private static AuditRecord BuildEvent(
            string operation = "GoldIntegration_JumpOff_Click",
            string details = "The user has jumped off to an integration partner")
        {
            return new AuditRecord
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
                Referrer = "ref1",
                ProviderId = "eConsult",
                ProviderName = "eConsult",
                JumpOffId = "onlineConsultation"
            };
        }
    }
}
