using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.PatientRecord;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisAllergyMapperTests
    {
        private IFixture _fixture;
        private IEmisAllergyMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisAllergyMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        } 

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WhenPassingNull_ThrowsNullReferenceException()
        {
            Action act = () => _mapper.Map((AllergyRequestsGetResponse)null);
            
            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("allergiesGetResponse");
        }   

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new AllergyRequestsGetResponse();

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Allergies.Should().BeEmpty();
        }

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new AllergyRequestsGetResponse
            {
                MedicalRecord = new AllergyMedicalRecord 
                {
                    Allergies = new List<AllergyResponse>
                    {
                        new AllergyResponse
                        {
                            Term = _fixture.Create<string>(),
                            AvailabilityDateTime = _fixture.Create<DateTimeOffset>(),
                        },
                    },
                }
            };

            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Allergies.Should().HaveCount(item.MedicalRecord.Allergies.Count());

            var expectedResult = new AllergyListResponse
            {
                Allergies = new List<AllergyItem>
                {
                    new AllergyItem
                    {
                        AllergyName = item.MedicalRecord.Allergies.ElementAt(0).Term,
                        AvailabilityDate = item.MedicalRecord.Allergies.ElementAt(0).AvailabilityDateTime,
                    }
                },
            };

            result.Should().BeEquivalentTo(expectedResult);
        }       
    }
}