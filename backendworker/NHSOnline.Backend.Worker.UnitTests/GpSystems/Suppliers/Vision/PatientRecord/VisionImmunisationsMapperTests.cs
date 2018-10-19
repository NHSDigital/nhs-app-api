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
    public class VisionImmunisationsMapperTests
    {
        IFixture _fixture;
        IVisionMapper<Immunisations> _mapper;
        ILogger<IVisionMapper<Immunisations>> _logger;

        const string VisionImmunisationsTestDataDirectory = "GpSystems/Suppliers/Vision/PatientRecord/TestData";

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<IVisionMapper<Immunisations>>>();
            _mapper = new VisionImmunisationsMapper(_logger);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Immunisations_Invalid_Immunisations_Response_Format()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = File.ReadAllText($"{VisionImmunisationsTestDataDirectory}/InvalidImmunisationsResponseContent.xml")
            };

            var expectedResult = new Immunisations
            {
                HasErrored = true
            };

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Immunisations_No_Immunisations_For_Patient()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = File.ReadAllText($"{VisionImmunisationsTestDataDirectory}/NoImmunisationResponseContent.xml")
            };

            var expectedResult = new Immunisations();

            // Act
            var actualResult = _mapper.Map(data);

            // Assert
            actualResult.Should().BeEquivalentTo(expectedResult);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Immunisations_From_Response_With_Immunisations()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = File.ReadAllText($"{VisionImmunisationsTestDataDirectory}/ValidImmunisationResponseContent.xml")
            };

            var expectedResult = new Immunisations
            {
                Data = new List<ImmunisationItem>
                {
                    new ImmunisationItem
                    {
                        Term = "Lumpectomy NEC",
                        EffectiveDate = new MyRecordDate
                        {
                            Value = DateTimeOffset.Parse("2018-10-10T00:00:00", CultureInfo.InvariantCulture),
                            DatePart = "Unknown"
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
