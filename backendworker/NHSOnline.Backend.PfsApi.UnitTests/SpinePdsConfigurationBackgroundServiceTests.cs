using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.NominatedPharmacy;
using NHSOnline.Backend.PfsApi.SpineSearch;
using UnitTestHelper;

namespace NHSOnline.Backend.PfsApi.UnitTests
{
    [TestClass]
    public class SpinePdsConfigurationBackgroundServiceTests
    {
        private IConfigurationBuilder configBuilder;

        private IFixture _fixture;
        private IConfiguration _configuration;
        private Mock<ILogger<SpinePdsConfigurationBackgroundService>> _logger;
        private Mock<ISpineSearchService> _spineSearchService;
        private Mock<INominatedPharmacyConfigurationSettings> _nominatedPharmacyConfigurationSettings;

        private SpinePdsConfigurationBackgroundService _systemUnderTest;

        private const int SpineLdapConnectionRetryWaitTimeSeconds = 1;
        private const string spinePdsUrl = "http://spine/";
        private const string spinePdsPath = "/action";
        private const string pdsFromAddress = "123.456";
        private const string pdsToAddress = "234.567";
        private const string nhsAppPartyId = "ab839";
        private const int delayAfterUpdate = 500;
        private const string bddTestDummyValue = "x";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<Mock<ILogger<SpinePdsConfigurationBackgroundService>>>();
            _spineSearchService = _fixture.Freeze<Mock<ISpineSearchService>>();

            configBuilder = new ConfigurationBuilder();
            configBuilder.AddInMemoryCollection(new[]
            {
                new KeyValuePair<string, string>(
                    "SPINE_LDAP_CONNECTION_RETRY_WAIT_TIME_SECONDS", $"{SpineLdapConnectionRetryWaitTimeSeconds}"),
                new KeyValuePair<string, string>(
                    "SPINE_LDAP_LOOKUP_ENABLED", "true"),
                new KeyValuePair<string, string>(
                    "NOMINATED_PHARMACY_ENABLED", "true"),
                new KeyValuePair<string, string>(
                    "SPINE_PDS_URL", spinePdsUrl),
                new KeyValuePair<string, string>(
                    "SPINE_PDS_URL_PATH", spinePdsPath),
                new KeyValuePair<string, string>(
                    "PDS_QUERY_FROM_ADDRESS", pdsFromAddress),
                new KeyValuePair<string, string>(
                    "PDS_QUERY_TO", pdsToAddress),
                new KeyValuePair<string, string>(
                    "DELAY_AFTER_NOMINATED_PHARMACY_UPDATE_IN_MILLISECONDS", $"{delayAfterUpdate}"),
                new KeyValuePair<string, string>(
                    "NHS_APP_PARTY_ID_FOR_SPINE", nhsAppPartyId),
                new KeyValuePair<string, string>(
                    "TEST_ONLY_SPINE_LDAP_LOOKUP_DUMMY_VALUE", bddTestDummyValue
                ),
            });

            _configuration = configBuilder.Build();
            _fixture.Inject(_configuration);
            _nominatedPharmacyConfigurationSettings = _fixture.Freeze<Mock<INominatedPharmacyConfigurationSettings>>();
            _systemUnderTest = _fixture.Create<SpinePdsConfigurationBackgroundService>();
        }

        [TestMethod]
        public async Task StartAsync_DoesNotEnableNominatedPharmacy_WhenLdapLookupEnabledIsFalseAndEnableNominatedPharmacyIsFalse()
        {
            // Arrange
            _configuration["SPINE_LDAP_LOOKUP_ENABLED"] = "false";
            _configuration["NOMINATED_PHARMACY_ENABLED"] = "false";

            // Act
            await _systemUnderTest.StartAsync(new CancellationToken());

            // Assert
            _spineSearchService.Verify(x => x.RetrieveSpinePropertiesForPdsTrace(), Times.Never());

            _nominatedPharmacyConfigurationSettings
                .Verify(
                    x => x.Update(It.IsAny<bool>(), It.IsAny<Uri>(), It.IsAny<string>(), It.IsAny<int>(), It.IsAny<PdsTraceConfigurationSettings>(), It.IsAny<PdsUpdateConfigurationSettings>()),
                    Times.Never());
        }

        [TestMethod]
        public async Task StartAsync_UsesTestLdapSettings_WhenLdapLookupEnabledIsFalseAndEnableNominatedPharmacyIsTrue()
        {
            // Arrange
            _configuration["SPINE_LDAP_LOOKUP_ENABLED"] = "false";
            _nominatedPharmacyConfigurationSettings.Setup(x => x.Validate()).Returns(true);

            // Act
            await _systemUnderTest.StartAsync(new CancellationToken());

            // Assert
            _nominatedPharmacyConfigurationSettings
                .Verify(
                    x => x.Update(true, It.Is<Uri>(u => string.Equals(u.AbsoluteUri, spinePdsUrl, StringComparison.Ordinal)), spinePdsPath, delayAfterUpdate,
                        It.Is<PdsTraceConfigurationSettings>(
                            pd => string.Equals(pd.FromAddress, pdsFromAddress, StringComparison.Ordinal)
                            && string.Equals(pd.ToAddress, pdsToAddress, StringComparison.Ordinal)
                            && string.Equals(pd.FromAsid, bddTestDummyValue, StringComparison.Ordinal)
                            && string.Equals(pd.ToAsid, bddTestDummyValue, StringComparison.Ordinal)),
                            It.Is<PdsUpdateConfigurationSettings>(pdu =>
                                string.Equals(pdu.FromAsid, bddTestDummyValue, StringComparison.Ordinal)
                                && string.Equals(pdu.ToAsid, bddTestDummyValue, StringComparison.Ordinal)
                                && string.Equals(pdu.CpaId, bddTestDummyValue, StringComparison.Ordinal)
                                && string.Equals(pdu.FromPartyId, nhsAppPartyId, StringComparison.Ordinal)
                                && string.Equals(pdu.ToPartyId, bddTestDummyValue, StringComparison.Ordinal))),
                    Times.Once);

            _nominatedPharmacyConfigurationSettings
                .VerifySet(x => x.IsNominatedPharmacyEnabled = false, Times.Never);
        }

        [TestMethod]
        public async Task StartAsync_CallsSpineSearchServiceAndEnablesNominatedPharmacy_WhenLdapLookupEnabledIsTrueAndConfigIsValid()
        {
            // Arrange
            var result = _fixture.Create<NhsAppSpinePdsUpdateProperties>();

            _nominatedPharmacyConfigurationSettings.Setup(x => x.Validate()).Returns(true);

            _spineSearchService
                .Setup(x => x.RetrieveSpinePropertiesForPdsUpdate())
                .Returns(result);

            // Act
            await _systemUnderTest.StartAsync(new CancellationToken());

            // Assert
            _nominatedPharmacyConfigurationSettings
                .Verify(
                    x => x.Update(true, It.Is<Uri>(u => string.Equals(u.AbsoluteUri, spinePdsUrl, StringComparison.Ordinal)), spinePdsPath, delayAfterUpdate,
                        It.Is<PdsTraceConfigurationSettings>(
                            pd => string.Equals(pd.FromAddress, pdsFromAddress, StringComparison.Ordinal)
                                  && string.Equals(pd.ToAddress, pdsToAddress, StringComparison.Ordinal)
                                  && string.Equals(pd.FromAsid, result.FromAsid, StringComparison.Ordinal)
                                  && string.Equals(pd.ToAsid, result.ToAsid, StringComparison.Ordinal)),
                        It.Is<PdsUpdateConfigurationSettings>(pdu =>
                            string.Equals(pdu.FromAsid, result.FromAsid, StringComparison.Ordinal)
                            && string.Equals(pdu.ToAsid, result.ToAsid, StringComparison.Ordinal)
                            && string.Equals(pdu.CpaId, result.CpaId, StringComparison.Ordinal)
                            && string.Equals(pdu.FromPartyId, nhsAppPartyId, StringComparison.Ordinal)
                            && string.Equals(pdu.ToPartyId, result.ToPartyId, StringComparison.Ordinal))),
                    Times.Once);

            _nominatedPharmacyConfigurationSettings
                .VerifySet(x => x.IsNominatedPharmacyEnabled = false, Times.Never);
        }

        [TestMethod]
        public async Task StartAsync_CallsSpineServiceRepeatedly_UntilItSuccessfullyReturnsValidResult()
        {
            // Arrange
            var emptyResult = new NhsAppSpinePdsUpdateProperties();
            var result = _fixture.Create<NhsAppSpinePdsUpdateProperties>();

            _nominatedPharmacyConfigurationSettings.Setup(x => x.Validate()).Returns(true);

            _spineSearchService
                .SetupSequence(x => x.RetrieveSpinePropertiesForPdsUpdate())
                .Returns(emptyResult) // 1st invocation
                .Returns(result); // 2nd invocation

            // Act
            await _systemUnderTest.StartAsync(new CancellationToken());

            // wait a length of time which will allow the retry to have ran twice (second call is successful)
            const int spineLdapConnectionRetryWaitTimeMilliseconds = SpineLdapConnectionRetryWaitTimeSeconds * 1000;
            await Task.Delay(spineLdapConnectionRetryWaitTimeMilliseconds * 2);

            // Assert
            _nominatedPharmacyConfigurationSettings
                .Verify(
                    x => x.Update(true, It.Is<Uri>(u => string.Equals(u.AbsoluteUri, spinePdsUrl, StringComparison.Ordinal)), spinePdsPath, delayAfterUpdate,
                        It.Is<PdsTraceConfigurationSettings>(
                            pd => string.Equals(pd.FromAddress, pdsFromAddress, StringComparison.Ordinal)
                                  && string.Equals(pd.ToAddress, pdsToAddress, StringComparison.Ordinal)
                                  && string.Equals(pd.FromAsid, result.FromAsid, StringComparison.Ordinal)
                                  && string.Equals(pd.ToAsid, result.ToAsid, StringComparison.Ordinal)),
                        It.Is<PdsUpdateConfigurationSettings>(pdu =>
                            string.Equals(pdu.FromAsid, result.FromAsid, StringComparison.Ordinal)
                            && string.Equals(pdu.ToAsid, result.ToAsid, StringComparison.Ordinal)
                            && string.Equals(pdu.CpaId, result.CpaId, StringComparison.Ordinal)
                            && string.Equals(pdu.FromPartyId, nhsAppPartyId, StringComparison.Ordinal)
                            && string.Equals(pdu.ToPartyId, result.ToPartyId, StringComparison.Ordinal))),
                    Times.Once);

            _spineSearchService.Verify(x => x.RetrieveSpinePropertiesForPdsUpdate(), Times.Exactly(2));

            _nominatedPharmacyConfigurationSettings
                .VerifySet(x => x.IsNominatedPharmacyEnabled = false, Times.Never);
        }

        [TestMethod]
        public async Task StartAsync_DisablesNominatedPharmacy_WhenValidateNominatedPharmacyConfigurationReturnsFalse()
        {
            // Arrange
            var result = _fixture.Create<NhsAppSpinePdsUpdateProperties>();
            _nominatedPharmacyConfigurationSettings.Setup(x => x.Validate()).Returns(false);

            _spineSearchService
                .Setup(x => x.RetrieveSpinePropertiesForPdsUpdate())
                .Returns(result);

            // Act
            await _systemUnderTest.StartAsync(new CancellationToken());

            // Assert
            _nominatedPharmacyConfigurationSettings
                .Verify(
                    x => x.Update(true, It.Is<Uri>(u => string.Equals(u.AbsoluteUri, spinePdsUrl, StringComparison.Ordinal)), spinePdsPath, delayAfterUpdate,
                        It.Is<PdsTraceConfigurationSettings>(
                            pd => string.Equals(pd.FromAddress, pdsFromAddress, StringComparison.Ordinal)
                                  && string.Equals(pd.ToAddress, pdsToAddress, StringComparison.Ordinal)
                                  && string.Equals(pd.FromAsid, result.FromAsid, StringComparison.Ordinal)
                                  && string.Equals(pd.ToAsid, result.ToAsid, StringComparison.Ordinal)),
                        It.Is<PdsUpdateConfigurationSettings>(pdu =>
                            string.Equals(pdu.FromAsid, result.FromAsid, StringComparison.Ordinal)
                            && string.Equals(pdu.ToAsid, result.ToAsid, StringComparison.Ordinal)
                            && string.Equals(pdu.CpaId, result.CpaId, StringComparison.Ordinal)
                            && string.Equals(pdu.FromPartyId, nhsAppPartyId, StringComparison.Ordinal)
                            && string.Equals(pdu.ToPartyId, result.ToPartyId, StringComparison.Ordinal))),
                    Times.Once);

            _nominatedPharmacyConfigurationSettings
                .VerifySet(x => x.IsNominatedPharmacyEnabled = false, Times.Once);

            _logger.VerifyLogger(LogLevel.Warning,
                $"Not all nominated pharmacy config is populated, disabling nominated pharmacy feature (initial value was True)", Times.Once());
        }
    }
}
