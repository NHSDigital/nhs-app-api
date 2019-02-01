using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Demographics.Models;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.Demographics
{
    [TestClass]
    public class EmisDemographicsMapperTests
    {
        private IFixture _fixture;
        private IEmisDemographicsMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisDemographicsMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_When_Passing_Null_Throws_ArgumentNullException()
        {
            Action act = () => _mapper.Map(null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("demographicsGetResponse");
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_Empty_Values_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new DemographicsGetResponse();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_Values_Returns_Result_Value()
        {
            // Arrange
            var item = new DemographicsGetResponse
            {
                Title = _fixture.Create<string>(),
                FirstName = _fixture.Create<string>(),
                Surname = _fixture.Create<string>(),
                DateOfBirth = _fixture.Create<DateTime>(),
                Sex = _fixture.Create<string>(),
                PatientIdentifiers = new List<PatientIdentifier>
                {
                    new PatientIdentifier
                    {
                        IdentifierType = IdentifierType.NhsNumber,
                        IdentifierValue = "1234567890"
                    }
                },
                Address = new EmisAddress
                {
                    HouseNameFlatNumber = _fixture.Create<string>(),
                    NumberStreet = _fixture.Create<string>(),
                    Village = _fixture.Create<string>(),
                    Town = _fixture.Create<string>(),
                    County = _fixture.Create<string>(),
                    Postcode = _fixture.Create<string>()
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            var expectedResult = new DemographicsResponse
            {
                PatientName = $"{item.Title} {item.FirstName} {item.Surname}",
                DateOfBirth = item.DateOfBirth,
                Sex = item.Sex,
                NhsNumber = "123 456 7890",
                Address = $"{item.Address.HouseNameFlatNumber}, {item.Address.NumberStreet}, {item.Address.Village}, " +
                          $"{item.Address.Town}, {item.Address.County}, {item.Address.Postcode}",
                AddressParts = new DemographicsAddress
                {
                    Text = $"{item.Address.HouseNameFlatNumber}, {item.Address.NumberStreet}, {item.Address.Village}, " +
                           $"{item.Address.Town}, {item.Address.County}",
                    Postcode = item.Address.Postcode
                },
                NameParts = new DemographicsName
                {
                    Title = item.Title,
                    Given = item.FirstName,
                    Surname = item.Surname
                },
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [DataTestMethod]
        [DataRow("Mr", "Fred", "Blogs", "Mr Fred Blogs")]
        [DataRow("", "Fred", "Blogs", "Fred Blogs")]
        [DataRow("Mr", "", "Blogs", "Mr Blogs")]
        [DataRow("Mr", "Fred", "", "Mr Fred")]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_MissingNameValues(string title, string firstname, string surname, string expectedName)
        {
            // Arrange
            var item = new DemographicsGetResponse
            {
                Title = title,
                FirstName = firstname,
                Surname = surname,
                DateOfBirth = _fixture.Create<DateTime>(),
                Sex = _fixture.Create<string>(),
                PatientIdentifiers = new List<PatientIdentifier>
                {
                    new PatientIdentifier
                    {
                        IdentifierType = IdentifierType.NhsNumber,
                        IdentifierValue = "1234567890"
                    }
                },
                Address = new EmisAddress
                {
                    HouseNameFlatNumber = _fixture.Create<string>(),
                    NumberStreet = _fixture.Create<string>(),
                    Village = _fixture.Create<string>(),
                    Town = _fixture.Create<string>(),
                    County = _fixture.Create<string>(),
                    Postcode = _fixture.Create<string>()
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            var expectedResult = new DemographicsResponse
            {
                PatientName = expectedName,
                DateOfBirth = item.DateOfBirth,
                Sex = item.Sex,
                NhsNumber = "123 456 7890",
                Address = $"{item.Address.HouseNameFlatNumber}, {item.Address.NumberStreet}, {item.Address.Village}, " +
                          $"{item.Address.Town}, {item.Address.County}, {item.Address.Postcode}",
                AddressParts = new DemographicsAddress
                {
                    Text = $"{item.Address.HouseNameFlatNumber}, {item.Address.NumberStreet}, {item.Address.Village}, " +
                           $"{item.Address.Town}, {item.Address.County}",
                    Postcode = item.Address.Postcode
                },
                NameParts = new DemographicsName
                {
                    Title = title,
                    Given = firstname,
                    Surname = surname
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
