using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord
{
    [TestClass]
    public class VisionAllergyMapperTests
    {
        private IFixture _fixture;
        private IVisionAllergyMapper _mapper;
        private ILogger<IVisionAllergyMapper> _logger;

        private const string VisionAllergyTestDataDirectory = "GpSystems/Suppliers/Vision/PatientRecord/TestData";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<IVisionAllergyMapper>>();
            _mapper = new VisionAllergyMapper(_logger);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Allergies_Invalid_Allergy_Response_Format()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = File.ReadAllText($"{VisionAllergyTestDataDirectory}/InvalidAllergyResponseContent.xml")
            };

            var expectedResult = new Allergies
            {
                HasErrored = true
            };

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Allergies_No_Allergies_For_Patient()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = File.ReadAllText($"{VisionAllergyTestDataDirectory}/NoAllergiesAllergyResponseContent.xml")
            };

            var expectedResult = new Allergies();

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Allergies_From_Response_With_Allergies()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = File.ReadAllText($"{VisionAllergyTestDataDirectory}/ValidAllergyResponseContent.xml")
            };

            var expectedResult = new Allergies
            {
                Data = new List<AllergyItem>
                {
                    new AllergyItem
                    {
                        Date = new MyRecordDate
                        {
                            Value = DateTimeOffset.Parse("2007-05-12T00:00:00", CultureInfo.InvariantCulture),
                            DatePart = "Unknown"
                        },
                        Name = "H/O: drug allergy",
                        Drug = "Paracetamol 500mg capsules",
                        Reaction = "Leg swelling"
                    },
                    new AllergyItem
                    {
                        Date = new MyRecordDate
                        {
                            Value = DateTimeOffset.Parse("2018-07-09T00:00:00", CultureInfo.InvariantCulture),
                            DatePart = "Unknown"
                        },
                        Name = "Strawberry allergy"
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
