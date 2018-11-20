using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Settings;
using NHSOnline.Backend.Worker.Support.Devices;
using static NHSOnline.Backend.Worker.Areas.Configuration.GetConfigurationResult;
using static NHSOnline.Backend.Worker.Constants;

namespace NHSOnline.Backend.Worker.UnitTests.Support.Devices
{
    [TestClass]
    public class SupportedDeviceServiceTests
    {
        private IFixture _fixture;

        private SupportedDeviceService _systemUnderTest;
        private Mock<ILogger<SupportedDeviceService>> _logger;
        private IOptions<ConfigurationSettings> _options;

        const string MinimumSupportedAndroidVersion = "2.1.0";
        const string MinimumSupportediOSVersion = "3.5.0";

        [TestInitialize]
        public void TestInitialise()
        {
            _options = Options.Create(new ConfigurationSettings
            {
                MinimumSupportedAndroidVersion = MinimumSupportedAndroidVersion,
                MinimumSupportediOSVersion = MinimumSupportediOSVersion
            });

            TestInitialiseWithOptions(_options);
        }

        private void TestInitialiseWithOptions(IOptions<ConfigurationSettings> options)
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger = _fixture.Freeze<Mock<ILogger<SupportedDeviceService>>>();
            _options = options;

            _fixture.Inject(_options);
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
            Assert.IsInstanceOfType(result, typeof(SuccessfullyRetrieved));
            var actualResult = (SuccessfullyRetrieved)result;
            Assert.AreEqual(expectedToBeValid, actualResult.Response.IsDeviceSupported);
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
            Assert.IsInstanceOfType(result, typeof(InvalidDeviceNameResult));
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
            Assert.IsInstanceOfType(result, typeof(SuccessfullyRetrieved));
            var actualResult = (SuccessfullyRetrieved)result;
            Assert.AreEqual(true, actualResult.Response.IsDeviceSupported);
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
            Assert.IsInstanceOfType(result, typeof(InvalidNativeAppVersionResult));
        }

        [TestMethod]
        public void IsDeviceSupported_ReturnsFalse_IfMinumumSupportedVersionFromConfigCantBeParsed()
        {
            // Arrange
            var deviceDetails = new DeviceDetails
            {
                Name = SupportedDeviceNames.Android,
                NativeAppVersion = MinimumSupportedAndroidVersion,
            };
            
            var options = Options.Create(new ConfigurationSettings
            {
                MinimumSupportedAndroidVersion = "1.blah",
            });

            // Reinitialise with new options
            TestInitialiseWithOptions(options);

            // Act
            var result = _systemUnderTest.IsDeviceSupported(deviceDetails);

            // Assert
            Assert.IsInstanceOfType(result, typeof(ErrorRetrievingConfigResult));
        }
    }
}