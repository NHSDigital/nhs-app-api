using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.PfsApi.SpineSearch;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Settings;
using Novell.Directory.Ldap;
using FluentAssertions;

namespace NHSOnline.Backend.PfsApi.UnitTests.SpineSearch
{
    [TestClass]
    public class SpineSearchServiceTests
    {
        private SpineSearchService _systemUnderTest;

        private Mock<ILogger<SpineSearchService>> _logger;
        private SpineLdapConfigurationSettings _settings;
        private Mock<ILdapConnectionService> _ldapConnectionService;

        private IFixture _fixture;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());

            _logger = new Mock<ILogger<SpineSearchService>>();
            _ldapConnectionService = new Mock<ILdapConnectionService>();
            _settings = _fixture.Create<SpineLdapConfigurationSettings>();

            _fixture.Inject(_logger);
            _fixture.Inject(_ldapConnectionService);
            _fixture.Inject(_settings);

            _systemUnderTest = _fixture.Create<SpineSearchService>();
        }

        [TestMethod]
        public void RetrieveSpinePropertiesForPdsTrace_MakesCorrectLdapRequests_AndInterpretsResponseCorrectly()
        {
            // Arrange
            var toAsid = _fixture.Create<string>();
            var fromAsid = _fixture.Create<string>();

            var toServiceAttributes = new LdapAttributeSet();
            toServiceAttributes.Add(new LdapAttribute("uniqueIdentifier", toAsid));

            var ldapConnection = new Mock<ILdapConnection>();

            _ldapConnectionService.Setup(x => x.CreateLdapConnection()).Returns(ldapConnection.Object);

            _ldapConnectionService
                .Setup(x => x.Search(
                    ldapConnection.Object,
                    _settings.LoginDN,
                    LdapConnection.ScopeOne,
                    "(&(nhsIDCode=YEA)(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pdsquery:QUPA_IN000008UK02))"
                ))
                .Returns(toServiceAttributes)
                .Verifiable();

            var fromServiceAttributes = new LdapAttributeSet();
            fromServiceAttributes.Add(new LdapAttribute("uniqueIdentifier", fromAsid));

            _ldapConnectionService
                .Setup(x => x.Search(
                    ldapConnection.Object,
                    _settings.LoginDN,
                    LdapConnection.ScopeOne,
                    $"(&(nhsMhsPartyKey={_settings.NhsAppPartyId})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pdsquery:QUPA_IN000008UK02))"
                ))
                .Returns(fromServiceAttributes)
                .Verifiable();

            // Act
            var result = _systemUnderTest.RetrieveSpinePropertiesForPdsTrace();

            // Assert
            result.FromAsid.Should().Be(fromAsid);
            result.ToAsid.Should().Be(toAsid);

            _ldapConnectionService.Verify(x => x.ConnectAndBind(ldapConnection.Object));
            _ldapConnectionService.Verify();
            ldapConnection.Verify(x => x.Disconnect());
        }

        [TestMethod]
        public void RetrieveSpinePropertiesForPdsUpdate_MakesCorrectLdapRequests_AndInterpretsResponseCorrectly()
        {
            // Arrange
            var toAsid = _fixture.Create<string>();
            var fromAsid = _fixture.Create<string>();
            var toPartyId = _fixture.Create<string>();
            var cpaId = _fixture.Create<string>();

            var ldapConnection = new Mock<ILdapConnection>();

            _ldapConnectionService.Setup(x => x.CreateLdapConnection()).Returns(ldapConnection.Object);

            ///

            var toServiceAttributes = new LdapAttributeSet();
            toServiceAttributes.Add(new LdapAttribute("uniqueIdentifier", toAsid));
            toServiceAttributes.Add(new LdapAttribute("nhsMHSPartyKey", toPartyId));

            _ldapConnectionService
                .Setup(x => x.Search(
                    ldapConnection.Object,
                    _settings.LoginDN,
                    LdapConnection.ScopeOne,
                    "(&(nhsIDCode=YEA)(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pds:PRPA_IN000203UK03))"
                ))
                .Returns(toServiceAttributes)
                .Verifiable();

            ///

            var cpaDetailAttributes = new LdapAttributeSet();
            cpaDetailAttributes.Add(new LdapAttribute("nhsMhsCPAId", cpaId));

            _ldapConnectionService
                .Setup(x => x.Search(
                    ldapConnection.Object,
                    _settings.LoginDN,
                    LdapConnection.ScopeOne,
                    "(&(nhsIDCode=YEA)(objectClass=nhsMHS)(nhsMHSSvcIA=urn:nhs:names:services:pds:PRPA_IN000203UK03))"
                ))
                .Returns(cpaDetailAttributes)
                .Verifiable();

            ///

            var fromSystemAttributes = new LdapAttributeSet();
            fromSystemAttributes.Add(new LdapAttribute("uniqueIdentifier", fromAsid));

            _ldapConnectionService
                .Setup(x => x.Search(
                    ldapConnection.Object,
                    _settings.LoginDN,
                    LdapConnection.ScopeOne,
                    $"(&(nhsMhsPartyKey={_settings.NhsAppPartyId})(objectClass=nhsAs)(nhsAsSvcIA=urn:nhs:names:services:pds:PRPA_IN000203UK03))"
                ))
                .Returns(fromSystemAttributes)
                .Verifiable();

            // Act
            var result = _systemUnderTest.RetrieveSpinePropertiesForPdsUpdate();

            // Assert
            result.FromAsid.Should().Be(fromAsid);
            result.ToAsid.Should().Be(toAsid);
            result.CpaId.Should().Be(cpaId);
            result.ToPartyId.Should().Be(toPartyId);

            _ldapConnectionService.Verify(x => x.ConnectAndBind(ldapConnection.Object));
            _ldapConnectionService.Verify();
            ldapConnection.Verify(x => x.Disconnect());
        }
    }
}
