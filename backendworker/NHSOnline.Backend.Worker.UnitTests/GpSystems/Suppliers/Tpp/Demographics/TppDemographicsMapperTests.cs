using System;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Demographics;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Tpp.Models;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Tpp.Demographics
{
    [TestClass]
    public class TppDemographicsMapperTests
    {
        private IFixture _fixture;
        private ITppDemographicsMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new TppDemographicsMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        }

        [TestMethod]
        public void Map_PatientSelectedReply_To_DemographicsResponse_When_Passing_Null_Throws_ArgumentNullException()
        {
            Action act = () => _mapper.Map(null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("patientSelectedReply");
        }

        [TestMethod]
        public void Map_PatientSelectedReply_To_DemographicsResponse_With_Empty_Values_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new PatientSelectedReply();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
        }

        [TestMethod]
        public void Map_PatientSelectedReply_To_DemographicsResponse_With_Values_Returns_Result_Value()
        {
            // Arrange
            var item = new PatientSelectedReply
            {
                  Person = new Person
                  {
                      Address = new TppAddress
                      {
                          Address = _fixture.Create<string>(),                      
                      },
                      DateOfBirth = _fixture.Create<DateTime>(),
                      Gender = _fixture.Create<string>(),
                      NationalId = new NationalId
                      {
                          Value = "1234567890"
                      },
                      PatientId = _fixture.Create<string>(),
                      PersonName = new PersonName
                      {
                          Name = _fixture.Create<string>()
                      }
                  }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            var expectedResult = new DemographicsResponse
            {
                PatientName = item.Person.PersonName.Name,
                DateOfBirth = item.Person.DateOfBirth,
                Sex = item.Person.Gender,
                NhsNumber = "123 456 7890",
                Address = item.Person.Address.Address,
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}
