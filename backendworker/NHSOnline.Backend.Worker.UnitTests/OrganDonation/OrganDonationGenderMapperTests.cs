using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.OrganDonation;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation
{
    [TestClass]
    public class OrganDonationGenderMapperTests
    {

        private IOrganDonationGenderMapper _organDonationGenderMapper;
        private Mock<IOrganDonationDataMaps> _dataMaps;
        
        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _dataMaps = fixture.Freeze<Mock<IOrganDonationDataMaps>>();

            _dataMaps
                .Setup(x => x.GenderDataMap)
                .Returns(new Dictionary<string, string>
                {
                    { "MALE", "male" },
                    { "FEMALE", "female" },
                    { "TRANSGENDER", "other" },
                    { "NOTKNOWN", "other" },
                    { "INDETERMINATE", "other" },
                    { "NOTSPECIFIED", "other" },
                    { "UNSPECIFIED", "other" }
                });

            _organDonationGenderMapper = fixture.Create<OrganDonationGenderMapper>();
        }

        [TestMethod]
        [DataRow("Male", "male")]
        [DataRow("Female", "female")]
        [DataRow("Transgender", "other")]
        [DataRow("NotKnown", "other")]
        [DataRow("Indeterminate", "other")]
        [DataRow("NotSpecified", "other")]
        [DataRow("Unspecified", "other")]
        [DataRow("", "unknown")]
        [DataRow("    ", "unknown")]
        public void MapOrganDonationGender_WhenPassingGender_MapsCorrectly(string gender, string mappedGender)
        {
            // Act
            var result = _organDonationGenderMapper.Map(gender);
            
            // Assert
            result.Should().Be(mappedGender);
        }
        
        [TestMethod]
        public void MapOrganDonationGender_WhenNotPassingGender_MapsCorrectly()
        {
            // Act
            var result = _organDonationGenderMapper.Map(null);
            
            // Assert
            result.Should().Be("unknown");
        }
        
    }
}