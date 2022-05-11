using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.Wayfinder.SecondaryCareSummary
{
    [TestClass]
    public class SecondaryCareSummaryEventParserTests
    {
        protected SecondaryCareSummaryEventParser SystemUnderTest;

        [TestInitialize]
        public void Setup()
        {
            SystemUnderTest = new SecondaryCareSummaryEventParser();
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
            parseResult.Should().BeOfType<SecondaryCareSummaryMetric>();
            parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
            parseResult.SessionId.Should().Be("SessionId-test");
            parseResult.TotalReferrals.Should().Be(123);
            parseResult.TotalUpcomingAppointments.Should().Be(456);
            parseResult.AuditId.Should().Be("AuditId-Test");
        }

        [TestMethod]
        public void Parse_MissingReferralsAndAppointments_ReferralsAndAppointmentsShouldBeZero()
        {
            // Arrange
            var source = BuildEventNoReferralsAndAppointments();

            // Act
            var parseResult = SystemUnderTest.Parse(source);

            // Assert
            parseResult.Should().BeOfType<SecondaryCareSummaryMetric>();
            parseResult.TotalReferrals.Should().Be(0);
            parseResult.TotalUpcomingAppointments.Should().Be(0);
        }

        private static AuditRecord BuildEvent(
            string operation = "SecondaryCare_GetSummary_Response",
            string details = "Secondary Care Summary successfully retrieved",
            int totalReferrals = 123,
            int totalUpcomingAppointments = 456)
        {
            return new AuditRecord()
            {
                AuditId = "AuditId-Test",
                NhsLoginSubject = "NhsLoginSubject-Test",
                NhsNumber = "NhsNumber-Test",
                IsActingOnBehalfOfAnother = false,
                Supplier = "Supplier-Test",
                Operation = operation,
                Details = $"{details} Total Referrals: {totalReferrals}, Total Upcoming Appointments: {totalUpcomingAppointments}",
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

        private static AuditRecord BuildEventNoReferralsAndAppointments(
            string operation = "SecondaryCare_GetSummary_Response",
            string details = "Secondary Care Summary successfully retrieved")
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

