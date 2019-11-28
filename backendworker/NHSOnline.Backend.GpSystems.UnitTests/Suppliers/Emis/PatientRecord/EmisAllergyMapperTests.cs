using System;
using System.Collections.Generic;
using System.Linq;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Emis.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Emis.PatientRecord
{
    [TestClass]
    public class EmisAllergyMapperTests
    {
        private IFixture _fixture;
        private IEmisMyRecordMapper _mapper;

        [TestInitialize]
        public void TestInitialize()
        {
            _mapper = new EmisMyRecordMapper();

            _fixture = new Fixture()
                .Customize(new AutoMoqCustomization());
        } 

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithNullResponse_ThrowsArgumentNullException()
        {
            Action act = () => new EmisAllergyMapper().Map(null);

            act.Should().Throw<ArgumentNullException>().And.ParamName.Should().Be("allergiesGetResponse");
        }

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new MedicationRootObject();

            // Act
            var result = _mapper.Map(new EmisAllergyMapper().Map(item), new Medications(), new Immunisations(), new TestResults(), new Problems(), new Consultations(), new PatientDocuments());

            // Assert
            result.Should().NotBeNull();
            result.Allergies.Data.Should().BeEmpty();
        }

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithValues_ReturnsResultValues()
        {
            // Arrange
            var item = new MedicationRootObject {
                MedicalRecord = new MedicalRecord
                {
                    Allergies = new List<Allergy>
                    {
                        new Allergy
                        {
                            Term = _fixture.Create<string>(),
                            EffectiveDate = new EffectiveDate
                            {
                                DatePart = "Unknown",
                                Value = _fixture.Create<DateTime>()
                            }
                        },
                    },
                }
            };
            
            // Act
            var result = new EmisAllergyMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(item.MedicalRecord.Allergies.Count);

            var expectedResult = new Allergies
            {
                Data = new List<AllergyItem>
                {
                    new AllergyItem
                    {
                        Name = item.MedicalRecord.Allergies.ElementAt(0).Term,
                        Date = new MyRecordDate { 
                            Value = item.MedicalRecord.Allergies.ElementAt(0).EffectiveDate.Value,
                            DatePart = item.MedicalRecord.Allergies.ElementAt(0).EffectiveDate.DatePart
                         }
                    }
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithNullEffectiveDate_ReturnsResultValuesWithEmptyDate()
        {
            // Arrange
            var item = new MedicationRootObject
            {
                MedicalRecord = new MedicalRecord
                {
                    Allergies = new List<Allergy>
                    {
                        new Allergy
                        {
                            Term = _fixture.Create<string>(),
                        },
                    },
                }
            };

            // Act
            var result = new EmisAllergyMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(item.MedicalRecord.Allergies.Count);

            var expectedResult = new Allergies
            {
                Data = new List<AllergyItem>
                {
                    new AllergyItem
                    {
                        Name = item.MedicalRecord.Allergies.ElementAt(0).Term,
                        Date = new MyRecordDate { }
                    }
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void MapAllergyRequestsGetResponseToAllergyListResponse_WithNullEffectiveDateValue_ReturnsResultValuesWithEmptyDate()
        {
            // Arrange
            var item = new MedicationRootObject
            {
                MedicalRecord = new MedicalRecord
                {
                    Allergies = new List<Allergy>
                    {
                        new Allergy
                        {
                            Term = _fixture.Create<string>(),
                            EffectiveDate = new EffectiveDate { }
                        },
                    },
                }
            };

            // Act
            var result = new EmisAllergyMapper().Map(item);

            // Assert
            result.Should().NotBeNull();
            result.Data.Should().HaveCount(item.MedicalRecord.Allergies.Count);

            var expectedResult = new Allergies
            {
                Data = new List<AllergyItem>
                {
                    new AllergyItem
                    {
                        Name = item.MedicalRecord.Allergies.ElementAt(0).Term,
                        Date = new MyRecordDate {
                            Value = item.MedicalRecord.Allergies.ElementAt(0).EffectiveDate.Value,
                            DatePart = item.MedicalRecord.Allergies.ElementAt(0).EffectiveDate.DatePart
                         }
                    }
                }
            };

            result.Should().BeEquivalentTo(expectedResult);
        }
    }
}