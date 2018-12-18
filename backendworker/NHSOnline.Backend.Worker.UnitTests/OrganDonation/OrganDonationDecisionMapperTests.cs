using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.Support;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationDecisionMapperTests
    {
        private IFixture _fixture;
        private IMapper<string, Decision> _organDonationDecisionMapper;

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
        [DataRow("app-rep")]
        public void MapStringToOrganDonationDecision_WhenPassingInvalidValue_MapsToDefault(string value)
        {
            // Act and Assert
            var result = _organDonationDecisionMapper.Map(value);


            result.Should().Be(default(Decision));
        }

        [TestMethod]
        [DataRow("opt-in", Decision.OptIn)]
        [DataRow("opt-out", Decision.OptOut)]
        public void MapStringToOrganDonationDecision_WhenPassingMappedValue_MapsCorrectly(string value,
            Decision expected)
        {
            // Act and Assert
            var result = _organDonationDecisionMapper.Map(value);


            result.Should().Be(expected);
        }
    }
}