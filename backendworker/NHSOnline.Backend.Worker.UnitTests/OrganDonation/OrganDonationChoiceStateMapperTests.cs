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
    public class OrganDonationChoiceStateMapperTests
    {
        private IFixture _fixture;
        private IEnumMapper<string, ChoiceState> _organDonationChoiceStateMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationChoiceStateMapper = _fixture.Create<OrganDonationChoiceStateMapper>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("_invalid_")]
        public void MapStringToOrganDonationChoiceState_WhenPassingInvalidValue_MapsToDefault(string value)
        {
            // Act and Assert
            var result = _organDonationChoiceStateMapper.To(value);


            result.Should().Be(default(ChoiceState));
        }

        [TestMethod]
        [DataRow("yes", ChoiceState.Yes)]
        [DataRow("no", ChoiceState.No)]
        [DataRow("not-stated", ChoiceState.NotStated)]
        public void MapStringToOrganDonationChoiceState_WhenPassingMappedValue_MapsCorrectly(string value,
            ChoiceState expected)
        {
            // Act and Assert
            var result = _organDonationChoiceStateMapper.To(value);


            result.Should().Be(expected);
        }
    }
}