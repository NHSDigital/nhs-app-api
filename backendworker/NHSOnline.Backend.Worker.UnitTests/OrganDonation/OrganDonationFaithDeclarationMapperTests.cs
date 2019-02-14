using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.OrganDonation.Models;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationFaithDeclarationMapperTests
    {
        private IFixture _fixture;
        private IEnumMapper<string, FaithDeclaration> _organDonationFaithDeclarationMapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _organDonationFaithDeclarationMapper = _fixture.Create<OrganDonationFaithDeclarationMapper>();
        }

        [TestMethod]
        [DataRow(null)]
        [DataRow("_invalid_")]
        public void MapStringToOrganDonationFaithDeclaration_WhenPassingInvalidValue_MapsToDefault(string value)
        {
            // Act and Assert
            var result = _organDonationFaithDeclarationMapper.To(value);


            result.Should().Be(default(FaithDeclaration));
        }

        [TestMethod]
        [DataRow("yes", FaithDeclaration.Yes)]
        [DataRow("no", FaithDeclaration.No)]
        [DataRow("not-stated", FaithDeclaration.NotStated)]
        public void MapStringToOrganDonationFaithDeclaration_WhenPassingMappedValue_MapsCorrectly(string value,
            FaithDeclaration expected)
        {
            // Act and Assert
            var result = _organDonationFaithDeclarationMapper.To(value);


            result.Should().Be(expected);
        }
    }
}