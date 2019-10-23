using System;
using System.Collections.Generic;
using System.Globalization;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.GpSystems.PatientRecord.Models;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;
using NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord
{
    [TestClass]
    public class VisionMedicationMapperTests
    {
        private IFixture _fixture;
        private IVisionMapper<Medications> _mapper;
        private ILogger<VisionMedicationMapper> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<VisionMedicationMapper>>();
            _mapper = new VisionMedicationMapper(_logger);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Medications_When_InvalidXml_ReturnsHasErrored()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = MedicationsTestData.GetInvalidMedicationTestData
            };

            var expectedResult = new Medications()
            {
                HasErrored = true,
                Data = new MedicationsData()
            };

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponseWithNoMedications_ToEmptyMedicatonsModel()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = MedicationsTestData.GetEmptyMedicationsDataResponse
            };

            var expectedResult = new Medications();

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_WithMedications_ToMedicationsModel()
        {
            // Arrange
            var today = DateTime.Now;

            var data = new VisionPatientDataResponse
            {
                Record = MedicationsTestData.GetMedicationTestData(today)
            };

            var expectedResult = new Medications()
            {
                Data = new MedicationsData()
                {
                    DiscontinuedRepeatMedications = new List<MedicationItem>()
                    {
                        new MedicationItem
                        {
                            Date = DateTime.Parse("2017-10-08T00:00:00", CultureInfo.InvariantCulture),
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem { Text = "Flucloxacillin 250mg capsules" },
                                new MedicationLineItem { Text = "1 TO 2 TABLETS UP TO FOUR TIMES DAILY AS REQUIRED" },
                                new MedicationLineItem { Text = "28 tablets" }
                            }
                            
                        },
                        new MedicationItem
                        {
                            Date = DateTime.Parse("2012-10-08T00:00:00", CultureInfo.InvariantCulture),
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem { Text = "Panadol ActiFast 50mg tablets" },
                                new MedicationLineItem { Text = "1 TABLET UP TO FIVE TIMES DAILY AS REQUIRED" },
                                new MedicationLineItem { Text = "14 capsules" }
                            }
                            
                        }
                    },
                    AcuteMedications = new List<MedicationItem>()
                    {
                        new MedicationItem
                        {
                            Date = DateTime.Parse(today.AddMonths(-10).Date.ToString(CultureInfo.InvariantCulture),
                                CultureInfo.InvariantCulture),
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem { Text = "Panadol ActiFast 500mg tablets (GlaxoSmithKline Consumer Healthcare)" },
                                new MedicationLineItem { Text = "FIVE TABLETS UP TO THREE TIMES DAILY AS REQUIRED" },
                                new MedicationLineItem { Text = "30 tablets" }
                            }
                            
                        },
                        new MedicationItem
                        {
                            Date = DateTime.Parse(today.AddMonths(-13).Date.ToString(CultureInfo.InvariantCulture),
                                CultureInfo.InvariantCulture),
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem { Text = "Flucloxacillin 100mg capsules" },
                                new MedicationLineItem { Text = "SIX TABLETS UP TO TWICE DAILY AS REQUIRED" },
                                new MedicationLineItem { Text = "80 capsules" }
                            }
                            
                        }
                    },
                    CurrentRepeatMedications = new List<MedicationItem>()
                    {
                        new MedicationItem
                        {
                            Date = DateTime.Parse("2015-10-08T00:00:00", CultureInfo.InvariantCulture),
                            LineItems = new List<MedicationLineItem>
                            {
                                new MedicationLineItem { Text = "Panadol ActiFast 1000mg tablets (GlaxoSmithKline Consumer Healthcare)" },
                                new MedicationLineItem { Text = "10 TABLETS UP TO TWO TIMES DAILY AS REQUIRED" },
                                new MedicationLineItem { Text = "45 tablets" }
                            }
                            
                        }
                    }
                }
            };

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }
    }
}
