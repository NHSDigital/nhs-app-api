using System;
using System.Threading;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.SpineSearch;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Settings;
using Novell.Directory.Ldap;

namespace NHSOnline.Backend.PfsApi.UnitTests.SpineSearch
{
    [TestClass]
    public class LdapConnectionServiceTests
    {
        private LdapConnectionService _systemUnderTest;

        private Mock<ILogger<LdapConnectionService>> _logger;
        private SpineLdapConfigurationSettings _spineLdapConfigurationSettings;
        private ConfigurationSettings _configurationSettings;

        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger = new Mock<ILogger<LdapConnectionService>>();
            _spineLdapConfigurationSettings = _fixture.Create<SpineLdapConfigurationSettings>();
            _configurationSettings = _fixture.Create<ConfigurationSettings>();

            _fixture.Inject(_logger);
            _fixture.Inject(_spineLdapConfigurationSettings);
            _fixture.Inject(_configurationSettings);
            _configurationSettings.DefaultHttpTimeoutSeconds = 1;

            _systemUnderTest = _fixture.Create<LdapConnectionService>();
        }

        [TestMethod]
        public void ConnectAndBind_CallsConnectAndBind_OnLdapConnection()
        {
            // Arrange
            var ldapConnection = new Mock<ILdapConnection>();

            // Act
            _systemUnderTest.ConnectAndBind(ldapConnection.Object);

            // Assert
            ldapConnection.Verify(x =>
                x.Connect(_spineLdapConfigurationSettings.LdapHost, _spineLdapConfigurationSettings.LdapPort));
            ldapConnection.Verify(x => x.Bind(_spineLdapConfigurationSettings.LoginDN, string.Empty));
        }

        [TestMethod]
        public void ConnectAndBind_ThrowsException_WhenConnectTakesLongerThanConfiguredTimeoutSetting()
        {
            // Arrange
            var ldapConnection = new Mock<ILdapConnection>();

            ldapConnection
                .Setup(x => x.Connect(It.IsAny<string>(), It.IsAny<int>()))
                .Callback(() => Thread.Sleep(2000)); // simulate the call taking 2 seconds

            // Act
            Action act = () => _systemUnderTest.ConnectAndBind(ldapConnection.Object);
            act.Should().Throw<TimeoutException>();

            // Assert
            ldapConnection.Verify(x =>
                x.Connect(_spineLdapConfigurationSettings.LdapHost, _spineLdapConfigurationSettings.LdapPort));
            ldapConnection.Verify(x => x.Bind(It.IsAny<string>(), It.IsAny<string>()), Times.Never);
        }
    }
}
