using System;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Demographics;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.Demographics
{
    [TestClass]
    public class VisionDemographicsMapperTests
    {
        private IVisionDemographicsMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new VisionDemographicsMapper();
        }

        [TestMethod]
        public void Map_PatientDemographics_To_DemographicsResponse_When_Passing_NullPatientDemographics_Throws_ArgumentNullException()
        {
            Action act = () => _mapper.Map(null, string.Empty);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("patientDemographics");
        }
        
        [TestMethod]
        public void Map_PatientDemographics_To_DemographicsResponse_When_Passing_NoNhsNumber_Throws_ArgumentNullException()
        {
            Action act = () => _mapper.Map(new VisionDemographics(), string.Empty);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("nhsNumber");
        }    

        [TestMethod]
        public void Map_PatientDemographics_To_DemographicsResponse_With_Empty_Values_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new VisionDemographics();
            const string testNhsNumber = "123456789";
            
            // Act
            var result = _mapper.Map(item, testNhsNumber);

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Map_PatientDemographics_To_DemographicsResponse_With_Values_Returns_Result_Value()
        {
            // Arrange
            var dateOfBirth = DateTime.Now.Date;
            const string testNhsNumber = "123456789";
            
            var item = new VisionDemographics
            {
                  Name = new Name { Title = "Mr", Forename = "Test", Surname = "Tester" },
                  PrimaryAddress =  new PrimaryAddress { HouseName = "The Lodge", HouseNumber = "12", Street = "Test Street", Town = "Test Town", Postcode = "L1 511",  },
                  DateOfBirth = dateOfBirth,                 
            };

            // Act
            var result = _mapper.Map(item, testNhsNumber);

            // Assert
            var expectedResult = new DemographicsResponse
            {
                PatientName = "Mr Test Tester",
                DateOfBirth = item.DateOfBirth,
                Sex = item.Gender?.Text,
                NhsNumber = testNhsNumber,
                Address = "The Lodge, 12 Test Street, Test Town, L1 511",
                AddressParts = new DemographicsAddress
                {
                    Text = "The Lodge, 12 Test Street, Test Town",
                    Postcode = "L1 511"
                },
                NameParts = new DemographicsName
                {
                    Title = "Mr",
                    Given = "Test",
                    Surname = "Tester"
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [TestMethod]
        public void Map_PatientDemographics_To_DemographicsResponse_HandlesAddressWithMissingTown()
        {
            // Arrange
            var dateOfBirth = DateTime.Now.Date;
            const string testNhsNumber = "123456789";
            
            var item = new VisionDemographics
            {
                Name = new Name { Title = "Mr", Forename = "Test", Surname = "Tester" },
                PrimaryAddress =  new PrimaryAddress { HouseName = "The Lodge", Street = "Test Street", Postcode = "L1 511",  },
                DateOfBirth = dateOfBirth,                 
            };

            // Act
            var result = _mapper.Map(item, testNhsNumber);

            // Assert
            var expectedResult = new DemographicsResponse
            {
                PatientName = "Mr Test Tester",
                DateOfBirth = item.DateOfBirth,
                Sex = item.Gender?.Text,
                NhsNumber = testNhsNumber,
                Address = "The Lodge, Test Street, L1 511",
                AddressParts = new DemographicsAddress
                {
                    Text = "The Lodge, Test Street",
                    Postcode = "L1 511"
                },
                NameParts = new DemographicsName
                {
                    Title = "Mr",
                    Given = "Test",
                    Surname = "Tester"
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
        
        [TestMethod]
        public void Map_PatientDemographics_To_DemographicsResponse_HandlesStreetWithHouseNumber()
        {
            // Arrange
            var dateOfBirth = DateTime.Now.Date;
            const string testNhsNumber = "123456789";
            
            var item = new VisionDemographics
            {
                Name = new Name { Title = "Mr", Forename = "Test", Surname = "Tester" },
                PrimaryAddress =  new PrimaryAddress { HouseName = "The Lodge", HouseNumber = "12", Street = "Test Street", Postcode = "L1 511",  },
                DateOfBirth = dateOfBirth,                 
            };

            // Act
            var result = _mapper.Map(item, testNhsNumber);

            // Assert
            var expectedResult = new DemographicsResponse
            {
                PatientName = "Mr Test Tester",
                DateOfBirth = item.DateOfBirth,
                Sex = item.Gender?.Text,
                NhsNumber = testNhsNumber,
                Address = "The Lodge, 12 Test Street, L1 511",
                AddressParts = new DemographicsAddress
                {
                    Text = "The Lodge, 12 Test Street",
                    Postcode = "L1 511"
                },
                NameParts = new DemographicsName
                {
                    Title = "Mr",
                    Given = "Test",
                    Surname = "Tester"
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}