using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Configuration;
using static NHSOnline.Backend.Support.Constants;
using NHSOnline.Backend.PfsApi.Devices;

namespace NHSOnline.Backend.PfsApi.UnitTests.Devices
{
    [TestClass]
    public class SupportedDeviceServiceTests
    {
        private IFixture _fixture;

        private SupportedDeviceService _systemUnderTest;
        private Mock<ILogger<SupportedDeviceService>> _logger;
        private DeviceConfigurationSettings _settings;

        private const string MinimumSupportedAndroidVersion = "2.1.0";
        private const string MinimumSupportediOSVersion = "3.5.0";

        private readonly Uri _testFidoServerUrl = new Uri("http://test.test.com");

        [TestInitialize]
        public void TestInitialize()
        {
            _settings = new DeviceConfigurationSettings
            {
                MinimumSupportedAndroidVersion = MinimumSupportedAndroidVersion,
                MinimumSupportediOSVersion = MinimumSupportediOSVersion,
                ThrottlingEnabled = "true",
                FidoServerUrl = _testFidoServerUrl
            };

            TestInitializeWithOptions(_settings);
        }

        private void TestInitializeWithOptions(DeviceConfigurationSettings settings)
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger = _fixture.Freeze<Mock<ILogger<SupportedDeviceService>>>();
            _settings = settings;

            _fixture.Inject(_settings);
            _fixture.Inject(_logger);

            _systemUnderTest = _fixture.Create<SupportedDeviceService>();
        }

        [DataTestMethod]
        [DataRow(SupportedDeviceNames.Android, "2.0.9239", false)] // lower
        [DataRow(SupportedDeviceNames.Android, "2.1.0", true)] // equal
        [DataRow(SupportedDeviceNames.Android, "2.1.1", true)] // higher
        [DataRow(SupportedDeviceNames.iOS, "3.4.912", false)] // lower
        [DataRow(SupportedDeviceNames.iOS, "3.5.0", true)] // equal
        [DataRow(SupportedDeviceNames.iOS, "3.5.1", true)] // higher
        public void IsDeviceSupported_ReturnsCorrectResult_ForDeviceAndVersion(string deviceName, string nativeAppVersion, bool expectedToBeValid)
        {
            // Arrange
            var deviceDetails = new DeviceDetails
            {
                Name = deviceName,
                NativeAppVersion = nativeAppVersion,
            };

            // Act
            var result = _systemUnderTest.IsDeviceSupported(deviceDetails);

            // Assert
            result.Should().BeOfType<GetConfigurationResult.Success>()
                .Subject.Response.IsDeviceSupported.Should().Be(expectedToBeValid);
        }

        [TestMethod]
        public void IsDeviceSupported_ReturnsFalse_IfDeviceNameIsNotRecognised()
        {
            // Arrange
            var deviceDetails = new DeviceDetails
            {
                Name = "made up",
                NativeAppVersion = MinimumSupportedAndroidVersion,
            };

            // Act
            var result = _systemUnderTest.IsDeviceSupported(deviceDetails);

            // Assert
            result.Should().BeOfType<GetConfigurationResult.BadRequest>();
        }

        [DataTestMethod]
        [DataRow("android", MinimumSupportedAndroidVersion)]
        [DataRow("ANDROID", MinimumSupportedAndroidVersion)]
        [DataRow("aNdRoId", MinimumSupportedAndroidVersion)]
        [DataRow("ios", MinimumSupportediOSVersion)]
        [DataRow("IOS", MinimumSupportediOSVersion)]
        [DataRow("iOs", MinimumSupportediOSVersion)]
        public void IsDeviceSupported_ReturnsTrue_IfDeviceNameIsNotInExpectedCase(string deviceName, string nativeAppVersion)
        {
            // Arrange
            var deviceDetails = new DeviceDetails
            {
                Name = deviceName,
                NativeAppVersion = nativeAppVersion,
            };

            // Act
            var result = _systemUnderTest.IsDeviceSupported(deviceDetails);

            // Assert
            result.Should().BeOfType<GetConfigurationResult.Success>()
                .Subject.Response.IsDeviceSupported.Should().BeTrue();
        }

        [TestMethod]
        public void IsDeviceSupported_ReturnsFalse_IfNativeAppVersionIsNotValid()
        {
            // Arrange
            var deviceDetails = new DeviceDetails
            {
                Name = SupportedDeviceNames.Android,
                NativeAppVersion = "1.blah",
            };

            // Act
            var result = _systemUnderTest.IsDeviceSupported(deviceDetails);

            // Assert
            result.Should().BeOfType<GetConfigurationResult.BadRequest>();
        }

        [TestMethod]
        public void IsDeviceSupported_ReturnsFalse_IfMinimumSupportedVersionFromConfigCantBeParsed()
        {
            // Arrange
            var deviceDetails = new DeviceDetails
            {
                Name = SupportedDeviceNames.Android,
                NativeAppVersion = MinimumSupportedAndroidVersion,
            };
            
            var settings = new DeviceConfigurationSettings
            {
                MinimumSupportedAndroidVersion = "1.blah",
                ThrottlingEnabled = "true",
                FidoServerUrl = _testFidoServerUrl
            };

            // Reinitialise with new options
            TestInitializeWithOptions(settings);

            // Act
            var result = _systemUnderTest.IsDeviceSupported(deviceDetails);

            // Assert
            result.Should().BeOfType<GetConfigurationResult.InternalServerError>();
        }
    }
}
