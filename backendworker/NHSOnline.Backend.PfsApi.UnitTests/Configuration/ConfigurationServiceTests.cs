using System;
using System.Collections.Generic;
using System.Security.Policy;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Azure.Documents.SystemFunctions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.Areas.Configuration;
using NHSOnline.Backend.PfsApi.Areas.Configuration.Models;
using NHSOnline.Backend.PfsApi.Configuration;
using NHSOnline.Backend.PfsApi.Devices;

namespace NHSOnline.Backend.PfsApi.UnitTests.Devices
{
    [TestClass]
    public class ConfigurationServiceTests
    {
        private IFixture _fixture;

        private Mock<ILogger<ConfigurationService>> _logger;
        private DeviceConfigurationSettings _settings;
        private Mock<IOptions<KnownServices>> _mockKnownServices;

        private const string MinimumSupportedAndroidVersion = "2.1.0";
        private const string MinimumSupportediOSVersion = "3.5.0";
        private KnownServices _knownServices;

        private readonly Uri _testFidoServerUrl = new Uri("http://test.test.com");
        private readonly Uri _testWebAppBaseUrl = new Uri("http://test.test.com");

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            
            _settings = new DeviceConfigurationSettings
            {
                MinimumSupportedAndroidVersion = MinimumSupportedAndroidVersion,
                MinimumSupportediOSVersion = MinimumSupportediOSVersion,
                FidoServerUrl = _testFidoServerUrl,
                ThrottlingEnabled = "true", 
                WebAppBaseUrl = _testWebAppBaseUrl
            };

            _logger = _fixture.Freeze<Mock<ILogger<ConfigurationService>>>();

            _fixture.Inject(_settings);
            _fixture.Inject(_logger);
            
            _knownServices = new KnownServices();
            _knownServices.Services = new List<KnownService>();
            _knownServices.Services.Add(new KnownService()
            {
                 AccessibleTitleKey = "titlekey",
                 AllowNativeInteraction = false,
                 IsExternal = true,
                 PathInfo = null,
                 Service = Service.Account,
                 TitleKey = "titlekey",
                 Url = new Uri("http://test.test.com"),
                 UseCustomTabs = true
            });

            _mockKnownServices = _fixture.Freeze<Mock<IOptions<KnownServices>>>();
            _mockKnownServices.Setup(x => x.Value)
                .Returns(_knownServices);
        }
                
        [TestMethod]
        public void GetConfiguration_Returns_CorrectResult()
        {
            // Arrange
            var expectedResult = new GetConfigurationResponseV2
            {
                MinimumSupportedAndroidVersion = MinimumSupportedAndroidVersion,
                MinimumSupportediOSVersion = MinimumSupportediOSVersion,
                FidoServerUrl = _testFidoServerUrl,
                KnownServices = _knownServices.Services
            };

            var systemUnderTest = _fixture.Create<ConfigurationService>();

            // Act
            var result = systemUnderTest.GetConfiguration();

            // Assert
            var response = result.Should().BeAssignableTo<GetConfigurationResultV2.Success>().Subject;
            response.Response.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Constructor_KnownServicesOptionsNull_ThrowsException()
        {
            Action act = () => new ConfigurationService(_logger.Object, null, _settings);

            act.Should().Throw<ArgumentNullException>();
        }

        [TestMethod]
        public void Constructor_KnownServicesOptionValuesNull_ThrowsException()
        {
            IOptions<KnownServices> knownServicesOptions = new OptionsWrapper<KnownServices>(null);
            Action act = () => new ConfigurationService(_logger.Object, knownServicesOptions, _settings);

            act.Should().Throw<ArgumentNullException>();
        }
        
        [TestMethod]
        public void Constructor_DeviceConfigurationNull_ThrowsException()
        {
            Action act = () => new ConfigurationService(_logger.Object, _mockKnownServices.Object, null);

            act.Should().Throw<ArgumentNullException>();
        }

        [DataTestMethod]
        [DataRow("http://www.test.com", null, "1.12.0", "http://www.test.com")]
        [DataRow("http://www.test.com", "   ", "1.12.0", "http://www.test.com")]
        [DataRow("http://www.test.com", "1.12.0", null, "http://www.test.com")] 
        [DataRow("http://www.test.com", "1.12.0", "   ", "http://www.test.com")] 
        [DataRow("http://www.test.com", "1.12.0", "1.12.0", null)] 
        [DataRow(null, "1.12.0", "1.12.0", "http://www.test.com")]
        public void Constructor_DeviceConfigurationMissingParameters_ThrowsException(
            string fidoServerUrl,
            string minimumSupportediOSVersion,
            string minimumSupportedAndroidVersion, string webAppBaseUrl)
        {
            var settings = new DeviceConfigurationSettings()
            {
                FidoServerUrl = string.IsNullOrWhiteSpace(fidoServerUrl) ? null : new Uri(fidoServerUrl),
                MinimumSupportediOSVersion = minimumSupportediOSVersion,
                MinimumSupportedAndroidVersion = minimumSupportedAndroidVersion,
                WebAppBaseUrl = string.IsNullOrWhiteSpace(webAppBaseUrl) ? null : new Uri(webAppBaseUrl),
            };
            
            Action act = () => new ConfigurationService(_logger.Object, _mockKnownServices.Object, settings);

            act.Should().Throw<ArgumentNullException>();
        }
    }
}