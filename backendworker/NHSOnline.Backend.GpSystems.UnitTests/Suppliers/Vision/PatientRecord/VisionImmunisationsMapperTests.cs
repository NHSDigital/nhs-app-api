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
    public class VisionImmunisationsMapperTests
    {
        private IFixture _fixture;
        private IVisionMapper<Immunisations> _mapper;
        private ILogger<VisionImmunisationsMapper> _logger;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<VisionImmunisationsMapper>>();
            _mapper = new VisionImmunisationsMapper(_logger);
        }

        [TestMethod]
        public void Map_VisionPatientDataResponse_To_Immunisations_Invalid_Immunisations_Response_Format()
        {
            // Arrange
            var data = new VisionPatientDataResponse
            {
                Record = ImmunisationsTestData.GetInvalidImmunisationsTestData
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
                Record = ImmunisationsTestData.GetEmptyImmunisationsDataResponse
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
                Record = ImmunisationsTestData.GetImmunisationsTestData
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
