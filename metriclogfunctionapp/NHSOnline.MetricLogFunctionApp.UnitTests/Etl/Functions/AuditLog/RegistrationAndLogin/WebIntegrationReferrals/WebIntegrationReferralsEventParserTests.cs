using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals
{
    [TestClass]
    public class WebIntegrationReferralsEventParserTests
    {
        protected WebIntegrationReferralsEventParser SystemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SystemUnderTest = new WebIntegrationReferralsEventParser();
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

        [DataTestMethod]
        [DataRow(null, DisplayName = "Null")]
        [DataRow("", DisplayName = "Blank")]
        public void Parse_NoIntegrationReferrer_ResultIsFalse(string integrationReferrer)
        {
            // Arrange
            var source = BuildEvent(integrationReferrer: integrationReferrer);

            // Act
            var parseResult = SystemUnderTest.Parse(source);

            // Assert
            parseResult.Should().BeNull();
        }

        [TestMethod]
        public void Parse_ValidInputs_OtherFieldsSetCorrectly()
        {
            // Arrange
            var source = BuildEvent();

            // Act
            var parseResult = SystemUnderTest.Parse(source);

            // Assert
            parseResult.Should().BeOfType<WebIntegrationReferralsMetric>();
            parseResult.Timestamp.Should().Be(new DateTimeOffset(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123)));
            parseResult.SessionId.Should().Be("SessionId-Test");
            parseResult.AuditId.Should().Be("AuditId-Test");
            parseResult.IntegrationReferrer.Should().Be("Int-Ref-Test");
        }

        private static AuditRecord BuildEvent(
            string operation = "Login_Success",
            string details = "Successful Login with",
            string integrationReferrer = "Int-Ref-Test")
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
                SessionId = "SessionId-Test",
                Timestamp = new DateTime(2021, 11, 01, 09, 00, 00, 123),
                ProofLevel = "P5",
                ODS = "Ods-Test",
                Referrer = "ref-Test",
                IntegrationReferrer = integrationReferrer
            };
        }
    }
}

