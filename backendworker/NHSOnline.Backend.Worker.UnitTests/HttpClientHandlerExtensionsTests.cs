using System;
using System.Net;
using System.Net.Http;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;

namespace NHSOnline.Backend.Worker.UnitTests
{
    [TestClass]
    public sealed class HttpClientHandlerExtensionsTests : IDisposable
    {
        private IFixture _fixture;
        private Mock<IConfiguration> _mockConfiguration;
        private HttpClientHandler _systemUnderTest;
        
        private const string HttpsProxyVariableLower = "https_proxy";
        private const string HttpsProxyVariableUpper = "HTTPS_PROXY";
        
        private const string NoProxyVariableLower = "no_proxy";
        private const string NoProxyVariableUpper = "NO_PROXY";

        private const string DummyHttpsProxySetting = "https://dummyproxy";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockConfiguration = _fixture.Freeze<Mock<IConfiguration>>();
            
            _systemUnderTest = new HttpClientHandler();
        }

        [TestMethod]
        public void ConfigureForwardProxy_HttpsProxyNotSpecified_ProxySettingsUnchanged()
        {
            // Arrange
            var originalUseProxy = _systemUnderTest.UseProxy;
            var originalProxy = _systemUnderTest.Proxy;

            // Act
            _systemUnderTest.ConfigureForwardProxy(_mockConfiguration.Object);

            // Assert
            _systemUnderTest.UseProxy.Should().Be(originalUseProxy);
            _systemUnderTest.Proxy.Should().Be(originalProxy);
        }
        
        [DataTestMethod]
        [DataRow(HttpsProxyVariableUpper, null)]
        [DataRow(HttpsProxyVariableUpper, "")]
        [DataRow(HttpsProxyVariableLower, null)]
        [DataRow(HttpsProxyVariableLower, "")]
        public void ConfigureForwardProxy_HttpsProxySettingNullOrEmpty_ProxySettingsUnchanged(string httpsProxyVariable, string httpsProxySetting)
        {
            // Arrange
            var originalUseProxy = _systemUnderTest.UseProxy;
            var originalProxy = _systemUnderTest.Proxy;
            _mockConfiguration.SetupGet(x => x[httpsProxyVariable]).Returns(httpsProxySetting);

            // Act
            _systemUnderTest.ConfigureForwardProxy(_mockConfiguration.Object);

            // Assert
            _systemUnderTest.UseProxy.Should().Be(originalUseProxy);
            _systemUnderTest.Proxy.Should().Be(originalProxy);
        }

        [DataTestMethod]
        [DataRow(HttpsProxyVariableLower)]
        [DataRow(HttpsProxyVariableUpper)]
        public void ConfigureForwardProxy_HttpsProxySpecified_ProxyUsed(string httpsProxyVariable)
        {
            // Arrange
            _mockConfiguration.SetupGet(x => x[httpsProxyVariable]).Returns(DummyHttpsProxySetting);
            
            // Act
            _systemUnderTest.ConfigureForwardProxy(_mockConfiguration.Object);
            
            // Assert
            _systemUnderTest.UseProxy.Should().Be(true);
            var actualProxy = _systemUnderTest.Proxy.Should().BeAssignableTo<WebProxy>().Subject;
            actualProxy.Address.Should().BeEquivalentTo(new Uri(DummyHttpsProxySetting));
            actualProxy.BypassList.Should().BeEmpty();
        }
       
        [DataTestMethod]
        [DataRow(NoProxyVariableUpper, null)]
        [DataRow(NoProxyVariableUpper, "")]
        [DataRow(NoProxyVariableLower, null)]
        [DataRow(NoProxyVariableLower, "")]
        public void ConfigureForwardProxy_NoProxySettingNullOrEmpty_BypassListEmpty(string noProxyVariable, string noProxySetting)
        {
            // Arrange
            _mockConfiguration.SetupGet(x => x[HttpsProxyVariableLower]).Returns(DummyHttpsProxySetting);
            _mockConfiguration.SetupGet(x => x[noProxyVariable]).Returns(noProxySetting);
            
            // Act
            _systemUnderTest.ConfigureForwardProxy(_mockConfiguration.Object);
            
            // Assert
            _systemUnderTest.UseProxy.Should().Be(true);
            var actualProxy = _systemUnderTest.Proxy.Should().BeAssignableTo<WebProxy>().Subject;
            actualProxy.Address.Should().BeEquivalentTo(new Uri(DummyHttpsProxySetting));
            actualProxy.BypassList.Should().BeEmpty();
        }
        
        [DataTestMethod]
        [DataRow(HttpsProxyVariableLower, NoProxyVariableLower)]
        [DataRow(HttpsProxyVariableUpper, NoProxyVariableLower)]
        [DataRow(HttpsProxyVariableLower, NoProxyVariableUpper)]
        [DataRow(HttpsProxyVariableUpper, NoProxyVariableUpper)]
        public void ConfigureForwardProxy_NoProxySpecified_BypassListPopulated(string httpsProxyVariable, string noProxyVariable)
        {
            // Arrange
            const string noProxyAddress = "www.google.com";
            _mockConfiguration.SetupGet(x => x[httpsProxyVariable]).Returns(DummyHttpsProxySetting);
            _mockConfiguration.SetupGet(x => x[noProxyVariable]).Returns(noProxyAddress);
            
            // Act
            _systemUnderTest.ConfigureForwardProxy(_mockConfiguration.Object);
            
            // Assert
            _systemUnderTest.UseProxy.Should().Be(true);
            var actualProxy = _systemUnderTest.Proxy.Should().BeAssignableTo<WebProxy>().Subject;
            actualProxy.Address.Should().BeEquivalentTo(new Uri(DummyHttpsProxySetting));
            actualProxy.BypassList.Should().BeEquivalentTo(noProxyAddress);
        }

        [TestMethod]
        public void ConfigureForwardProxy_MultipleEntriesInNoProxySetting_BypassListIncludesAllEntries()
        {
            // Arrange
            const string noProxyAddress = ".google.com .bbc.co.uk .apple.com";
            _mockConfiguration.SetupGet(x => x[HttpsProxyVariableLower]).Returns(DummyHttpsProxySetting);
            _mockConfiguration.SetupGet(x => x[NoProxyVariableLower]).Returns(noProxyAddress);
            
            // Act
            _systemUnderTest.ConfigureForwardProxy(_mockConfiguration.Object);
            
            // Assert
            _systemUnderTest.UseProxy.Should().Be(true);
            var actualProxy = _systemUnderTest.Proxy.Should().BeAssignableTo<WebProxy>().Subject;
            actualProxy.Address.Should().BeEquivalentTo(new Uri(DummyHttpsProxySetting));
            actualProxy.BypassList.Should()
                .BeEquivalentTo(".google.com", ".bbc.co.uk", ".apple.com");
        }

        [TestMethod]
        public void ConfigureForwardProxy_NoProxySettingContainsLeadingTrailingOrMultipleSpaces_BypassListIgnoresThese()
        {
            // Arrange
            const string noProxyAddress = " .google.com .bbc.co.uk    .apple.com ";
            _mockConfiguration.SetupGet(x => x[HttpsProxyVariableLower]).Returns(DummyHttpsProxySetting);
            _mockConfiguration.SetupGet(x => x[NoProxyVariableLower]).Returns(noProxyAddress);
            
            // Act
            _systemUnderTest.ConfigureForwardProxy(_mockConfiguration.Object);
            
            // Assert
            _systemUnderTest.UseProxy.Should().Be(true);
            var actualProxy = _systemUnderTest.Proxy.Should().BeAssignableTo<WebProxy>().Subject;
            actualProxy.Address.Should().BeEquivalentTo(new Uri(DummyHttpsProxySetting));
            actualProxy.BypassList.Should()
                .BeEquivalentTo(".google.com", ".bbc.co.uk", ".apple.com");
        }

        public void Dispose()
        {
            _systemUnderTest?.Dispose();
        }
    }
}