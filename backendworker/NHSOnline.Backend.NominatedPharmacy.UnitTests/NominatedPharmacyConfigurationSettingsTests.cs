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
            string spineAccreditedSystemIdFrom = _fixture.Create<string>();
            string spineAccreditedSystemIdTo = _fixture.Create<string>();
            string pdsQueryFromAddress = _fixture.Create<string>();
            string pdsQueryToAddress = _fixture.Create<string>();
            int artificialDelayAfterNominatedPharmacyUpdateInMilliseconds = _fixture.Create<int>();
            string partyIdFrom = _fixture.Create<string>();
            string partyIdTo = _fixture.Create<string>();

            // Act
            var config = new NominatedPharmacyConfigurationSettings(
                isNominatedPharmacyEnabled,
                baseUrl,
                spineAccreditedSystemIdFrom,
                spineAccreditedSystemIdTo,
                pdsQueryFromAddress,
                pdsQueryToAddress,
                artificialDelayAfterNominatedPharmacyUpdateInMilliseconds,
                partyIdFrom,
                partyIdTo);

            // Assert
            config.IsNominatedPharmacyEnabled.Should().Be(isNominatedPharmacyEnabled);
            config.BaseUrl.Should().BeEquivalentTo(baseUrl);
            config.SpineAccreditedSystemIdFrom.Should().Be(spineAccreditedSystemIdFrom);
            config.SpineAccreditedSystemIdTo.Should().Be(spineAccreditedSystemIdTo);
            config.PdsQueryFromAddress.Should().Be(pdsQueryFromAddress);
            config.PdsQueryTo.Should().Be(pdsQueryToAddress);
            config.PartyIdFrom.Should().Be((partyIdFrom));
            config.PartyIdTo.Should().Be((partyIdTo));
        }
    }
}
