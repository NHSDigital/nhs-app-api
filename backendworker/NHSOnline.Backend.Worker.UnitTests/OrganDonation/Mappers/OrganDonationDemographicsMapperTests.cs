using System;
using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.Worker.OrganDonation;
using NHSOnline.Backend.Worker.OrganDonation.Models;

namespace NHSOnline.Backend.Worker.UnitTests.OrganDonation.Mappers
{
    [TestClass]
    public class OrganDonationDemographicsMapperTests
    {
        private IFixture _fixture;
        private Mock<IOrganDonationDataMaps> _mockDataMaps;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void MapStringAndDemographicsNameToOrganDonationName_WithNameParts_MapsCorrectly()
        {
            //Arrange
            var title = _fixture.Create<string>();
            var firstName = _fixture.Create<string>();
            var lastName = _fixture.Create<string>();
            var patientName = _fixture.Create<string>();
            
            var nameParts = new DemographicsName()
            {
                Given = firstName,
                Surname = lastName,
                Title = title
            };
            
            var expectedResult = new Name()
            {
                Title = title,
                GivenName = firstName,
                Surname = lastName
            };

            _mockDataMaps = _fixture.Freeze<Mock<IOrganDonationDataMaps>>();
            ConfigureTitleDataMaps(title, title);
            
            var systemUnderTest = _fixture.Create<OrganDonationDemographicsNameMapper>();

            //Act
            var result = systemUnderTest.Map(patientName, nameParts);
            
            //Assert
            _mockDataMaps.Verify();
            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [TestMethod]
        public void MapStringAndDemographicsNameToOrganDonationName_WithNamePartsAndStringName_MapsCorrectly()
        {
            //Arrange
            var title = _fixture.Create<string>();
            var firstName = _fixture.Create<string>();
            var lastName = _fixture.Create<string>();

            var patientName = $"{title} {firstName} {lastName}";
            
            var nameParts = new DemographicsName()
            {
                Given = firstName,
                Surname = lastName,
                Title = title
            };
            
            var expectedResult = new Name()
            {
                Title = title,
                GivenName = firstName,
                Surname = lastName
            };
            
            _mockDataMaps = _fixture.Freeze<Mock<IOrganDonationDataMaps>>();
            
            var systemUnderTest = _fixture.Create<OrganDonationDemographicsNameMapper>();

            //Act
            var result = systemUnderTest.Map(patientName, nameParts);
            
            //Assert
            _mockDataMaps.Verify();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapStringAndDemographicsNameToOrganDonationName_WithNoNamePartsAndStringName_MapsCorrectly()
        {
            //Arrange
            var title = _fixture.Create<string>();
            var firstName = _fixture.Create<string>();
            var lastName = _fixture.Create<string>();
            var expectedTitle = _fixture.Create<string>();

            var patientName = $"{title} {firstName} {lastName}";
            
            var expectedResult = new Name()
            {
                Title = expectedTitle,
                GivenName = firstName,
                Surname = lastName
            };

            _mockDataMaps = _fixture.Freeze<Mock<IOrganDonationDataMaps>>();
            ConfigureTitleDataMaps(title, expectedTitle);
            
            var systemUnderTest = _fixture.Create<OrganDonationDemographicsNameMapper>();

            //Act
            var result = systemUnderTest.Map(patientName, null);
            
            //Assert
            _mockDataMaps.Verify();
            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapStringAndDemographicsNameToOrganDonationName_WithNoDemographicsNameAndNoStringName_ThrowsArgumentNullException()
        {
            
            var systemUnderTest = _fixture.Create<OrganDonationDemographicsNameMapper>();
            // Act and Assert
            Action act = () => systemUnderTest.Map(null, null);

            act.Should().Throw<ArgumentException>().And.Message.Should().Contain("firstSource");
            act.Should().Throw<ArgumentException>().And.Message.Should().Contain("secondSource");
            
        }

        private void ConfigureTitleDataMaps(string title, string expectedTitle)
        {
            _mockDataMaps
                .Setup(x => x.TitleDataMap)
                .Returns(new Dictionary<string, string>()
                {   
                    {title.ToUpper(CultureInfo.InvariantCulture),expectedTitle}
                })
                .Verifiable();
        }
    }
}