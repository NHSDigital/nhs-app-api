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
    public class VisionProblemsMapperTests
    {
        private IFixture _fixture;
        private VisionProblemsMapper _mapper;
        private ILogger<VisionProblemsMapper> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<VisionProblemsMapper>>();
            _mapper = new VisionProblemsMapper(_logger);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Problems_Invalid_Immunisations_Response_Format()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = ProblemsTestData.GetInvalidProblemsTestData
            };

            var expectedResult = new Problems
            {
                HasErrored = true
            };

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Problems_No_Problems_For_Patient()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = ProblemsTestData.GetEmptyProblemsDataResponse
            };

            var expectedResult = new Problems();

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Problems_From_Response_With_Problems()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = ProblemsTestData.GetProblemsTestData
            };

            var expectedResult = new Problems
            {
                Data = new List<ProblemItem>
                {
                    new ProblemItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = DateTimeOffset.Parse("2018-10-10T00:00:00", CultureInfo.InvariantCulture),
                            DatePart = "Unknown"
                        },
                        LineItems = new List<ProblemLineItem>
                        {
                            new ProblemLineItem { Text = "Peanut allergy" },
                            new ProblemLineItem { Text = "Status: Past" }
                        }
                    },
                    new ProblemItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = DateTimeOffset.Parse("2018-10-10T00:00:00", CultureInfo.InvariantCulture),
                            DatePart = "Unknown"
                        },
                        LineItems = new List<ProblemLineItem>
                        {
                            new ProblemLineItem { Text = "Broken leg" },
                            new ProblemLineItem { Text = "Status: Current" }
                        }
                    },
                    new ProblemItem
                    {
                        EffectiveDate = new MyRecordDate
                        {
                            Value = DateTimeOffset.Parse("2018-10-10T00:00:00", CultureInfo.InvariantCulture),
                            DatePart = "Unknown"
                        },
                        LineItems = new List<ProblemLineItem>
                        {
                            new ProblemLineItem { Text = "Acne" },
                            new ProblemLineItem { Text = "Status: Random" }
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
