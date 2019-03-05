using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.PfsApi.OrganDonation.Models;
using NHSOnline.Backend.PfsApi.OrganDonation.Mappers;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.UnitTests.OrganDonation.Mappers
{
    [TestClass]
    public class OrganDonationDecisionMapperTests
    {
        private IFixture _fixture;
        private IEnumMapper<string, Decision> _organDonationDecisionMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationDecisionMapper = _fixture.Create<OrganDonationDecisionMapper>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("_invalid_")]
        public void MapStringToOrganDonationDecision_WhenPassingInvalidValue_MapsToDefault(string value)
        {
            // Act and Assert
            var result = _organDonationDecisionMapper.To(value);


            result.Should().Be(default(Decision));
        }

        [TestMethod]
        [DataRow("opt-in", Decision.OptIn)]
        [DataRow("opt-out", Decision.OptOut)]
        [DataRow("app-rep", Decision.AppRep)]
        public void MapStringToOrganDonationDecision_WhenPassingMappedValue_MapsCorrectly(string value,
            Decision expected)
        {
            // Act and Assert
            var result = _organDonationDecisionMapper.To(value);


            result.Should().Be(expected);
        }
    }
}