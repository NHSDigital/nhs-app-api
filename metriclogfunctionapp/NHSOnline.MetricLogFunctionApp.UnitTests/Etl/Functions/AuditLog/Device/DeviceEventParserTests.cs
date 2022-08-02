using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.Device;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Etl.Functions.AuditLog.Device
{
    [TestClass]
    public class DeviceEventParserTests
    {
        protected DeviceEventParser SystemUnderTest;
        const string MobileUserAgent = "nhsapp-ios/AppVersion-Test nhsapp-manufacturer/DeviceManufacturer-Test " +
                        "nhsapp-model/DeviceModel-Test nhsapp-os/DeviceOSVersion-Test nhsapp-architecture/arm64";
        const string NonMobileUserAgent = "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) " +
                                 "Chrome/93.0.4577.63 Safari/537.36";

        [TestInitialize]
        public void Setup()
        {
            SystemUnderTest = new DeviceEventParser();
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
        public void Parse_ValidInputs_OtherFieldsSetCorrectlyForMobileDevice()
        {
            // Arrange
            var source = BuildEvent();

            // Act
            var parseResult = SystemUnderTest.Parse(source);

            // Assert
            parseResult.Should().BeOfType<DeviceMetric>();
            parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
            parseResult.SessionId.Should().Be("SessionId-test");
            parseResult.AppVersion.Should().Be("AppVersion-Test");
            parseResult.DeviceManufacturer.Should().Be("DeviceManufacturer-Test");
            parseResult.DeviceModel.Should().Be("DeviceModel-Test");
            parseResult.DeviceOS.Should().Be("ios");
            parseResult.DeviceOSVersion.Should().Be("DeviceOSVersion-Test");
            parseResult.UserAgent.Should().Be(MobileUserAgent);
            parseResult.AuditId.Should().Be("AuditId-Test");
        }

        [TestMethod]
        public void Parse_ValidInputs_DetailsFieldsForNonMobileDevice()
        {
            // Arrange
            var source = BuildEvent(details: $"Device details returned: {NonMobileUserAgent}");

            // Act
            var parseResult = SystemUnderTest.Parse(source);

            // Assert
            parseResult.Should().BeOfType<DeviceMetric>();
            parseResult.Timestamp.Should().Be(new DateTime(2021, 11, 1).AddHours(9).AddMilliseconds(123));
            parseResult.SessionId.Should().Be("SessionId-test");
            parseResult.AppVersion.Should().BeNull();
            parseResult.DeviceManufacturer.Should().BeNull();
            parseResult.DeviceModel.Should().BeNull();
            parseResult.DeviceOS.Should().BeNull();
            parseResult.DeviceOSVersion.Should().BeNull();
            parseResult.UserAgent.Should().Be(NonMobileUserAgent);
            parseResult.AuditId.Should().Be("AuditId-Test");
        }

        private static AuditRecord BuildEvent(
            string operation = "Login_Device",
            string details = $"Device details returned: {MobileUserAgent}")
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
