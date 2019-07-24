using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace NHSOnline.Backend.NominatedPharmacy.UnitTests
{
    [TestClass]
    public class NominatedPharmacyConfigurationSettingsTests
    {
        private IFixture _fixture;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void NominatedPharmacyConfigurationSettings_SetsValuesCorrectlyFromConstructor()
        {
            // Arrange
            bool isNominatedPharmacyEnabled = true;
            Uri baseUrl = new Uri("http://test-url/nom");
            string pdsTraceAccreditedSystemIdFrom = _fixture.Create<string>();
            string pdsTraceAccreditedSystemIdTo = _fixture.Create<string>();
            string pdsUpdateAccreditedSystemIdFrom = _fixture.Create<string>();
            string pdsUpdateAccreditedSystemIdTo = _fixture.Create<string>();
            string cpaId = _fixture.Create<string>();
            string pdsQueryFromAddress = _fixture.Create<string>();
            string pdsQueryToAddress = _fixture.Create<string>();
            int artificialDelayAfterNominatedPharmacyUpdateInMilliseconds = _fixture.Create<int>();
            string partyIdFrom = _fixture.Create<string>();
            string partyIdTo = _fixture.Create<string>();

            var pdsTraceConfigurationSettings = new PdsTraceConfigurationSettings
            {
                FromAddress = pdsQueryFromAddress,
                ToAddress = pdsQueryToAddress,
                FromAsid = pdsTraceAccreditedSystemIdFrom,
                ToAsid = pdsTraceAccreditedSystemIdTo,
            };

            var pdsUpdateConfigurationSettings = new PdsUpdateConfigurationSettings
            {
                FromAsid = pdsUpdateAccreditedSystemIdFrom,
                ToAsid = pdsUpdateAccreditedSystemIdTo,
                CpaId = cpaId,
                FromPartyId = partyIdFrom,
                ToPartyId = partyIdTo,
            };

            // Act
            var config = new NominatedPharmacyConfigurationSettings(
                isNominatedPharmacyEnabled,
                baseUrl,
                artificialDelayAfterNominatedPharmacyUpdateInMilliseconds,
                pdsTraceConfigurationSettings,
                pdsUpdateConfigurationSettings);

            // Assert
            config.IsNominatedPharmacyEnabled.Should().Be(isNominatedPharmacyEnabled);
            config.BaseUrl.Should().BeEquivalentTo(baseUrl);

            config.PdsTraceConfigurationSettings.FromAddress.Should().Be(pdsQueryFromAddress);
            config.PdsTraceConfigurationSettings.ToAddress.Should().Be(pdsQueryToAddress);
            config.PdsTraceConfigurationSettings.FromAsid.Should().Be(pdsTraceAccreditedSystemIdFrom);
            config.PdsTraceConfigurationSettings.ToAsid.Should().Be(pdsTraceAccreditedSystemIdTo);

            config.PdsUpdateConfigurationSettings.FromAsid.Should().Be(pdsUpdateAccreditedSystemIdFrom);
            config.PdsUpdateConfigurationSettings.ToAsid.Should().Be(pdsUpdateAccreditedSystemIdTo);
            config.PdsUpdateConfigurationSettings.FromPartyId.Should().Be(partyIdFrom);
            config.PdsUpdateConfigurationSettings.ToPartyId.Should().Be(partyIdTo);
            config.PdsUpdateConfigurationSettings.CpaId.Should().Be(cpaId);

        }
    }
}
