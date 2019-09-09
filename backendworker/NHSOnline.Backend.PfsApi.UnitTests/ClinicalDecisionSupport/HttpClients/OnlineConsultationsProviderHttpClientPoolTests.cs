using System.Collections.Generic;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Settings;

namespace NHSOnline.Backend.PfsApi.UnitTests.ClinicalDecisionSupport.HttpClients
{
    [TestClass]
    public class OnlineConsultationsProviderHttpClientPoolTests
    {
        private readonly Mock<ILoggerFactory> _mockLoggerFactory = new Mock<ILoggerFactory>();

        private readonly Mock<ILogger<OnlineConsultationsProviderHttpClientPool>> _mockLogger =
            new Mock<ILogger<OnlineConsultationsProviderHttpClientPool>>();
        
        private OnlineConsultationsProvidersSettings _providersSettings;
        private readonly OnlineConsultationsProviderSettings ProviderOneSettings = new OnlineConsultationsProviderSettings
        {
            Provider = "ProviderOne",
            BaseAddress = "http://test.test/one/",
            BearerToken = "testBearerToken1"
        };
        private readonly OnlineConsultationsProviderSettings ProviderTwoSettings = new OnlineConsultationsProviderSettings
        {
            Provider = "ProviderTwo",
            BaseAddress = "http://test.test/two/",
            BearerToken = "testBearerToken2"
        };
        private readonly OnlineConsultationsProviderSettings ProviderThreeSettings = new OnlineConsultationsProviderSettings
        {
            Provider = "ProviderThree",
            BaseAddress = "http://test.test/three/",
            BearerToken = "testBearerToken3"
        };

        private OnlineConsultationsProviderHttpClientPool _pool;
            
        [TestInitialize]
        public void TestInitialize()
        {
            _providersSettings = new OnlineConsultationsProvidersSettings
            {
                Providers = new List<OnlineConsultationsProviderSettings>{ ProviderOneSettings, ProviderThreeSettings, ProviderTwoSettings }
            };

            _pool = new OnlineConsultationsProviderHttpClientPool(
                _providersSettings,
                _mockLoggerFactory.Object,
                _mockLogger.Object);
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("UnknownProvider")]
        public void GetClientByProviderName_WhenInvokedWithNullOrUnknownProviderName_ReturnsNull(string providerName)
        {
            // Act
            var providerClient = _pool.GetClientByProviderName(providerName);
            
            // Assert
            providerClient.Should().Be(null);
        }

        [TestMethod]
        [DataRow("ProviderTwo")]
        [DataRow("ProviderOne")]
        [DataRow("ProviderThree")]
        public void GetClientByProviderName_WhenInvokedWithKnownProviderName_ReturnsCorrespondingProviderHttpClient(string providerName)
        {
            // Act
            var providerClient = _pool.GetClientByProviderName(providerName);

            // Assert
            providerClient.Should().NotBe(null);
        }

        [TestMethod]
        public void OnlineConsultationsProviderHttpClientPool_WhenCreated_CreatesProviderHttpClientForEachProviderSettings()
        {
            _pool.GetClientByProviderName(ProviderOneSettings.Provider).Should().NotBeNull();
            _pool.GetClientByProviderName(ProviderTwoSettings.Provider).Should().NotBeNull();
            _pool.GetClientByProviderName(ProviderThreeSettings.Provider).Should().NotBeNull();
        }
    }
}