using System;
using System.Collections.Generic;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.Demographics.Models;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
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
            result.Address.Should().NotBeNull();
        }

        [TestMethod]
        public void Map_DemographicsGetResponse_To_DemographicsResponse_With_Values_Returns_Result_Value()
        {
            // Arrange
            var item = new DemographicsGetResponse
            {
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
                FirstName = item.FirstName,
                Surname = item.Surname,
                DateOfBirth = item.DateOfBirth,
                Sex = item.Sex,
                NhsNumber = "123 456 7890",
                Address = new Address
                {
                    Line1 = item.Address.HouseNameFlatNumber,
                    Line2 = item.Address.NumberStreet,
                    Line3 = item.Address.Village,
                    Town = item.Address.Town,
                    County = item.Address.County,
                    Postcode = item.Address.Postcode
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
