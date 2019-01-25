using System;
using System.IO;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord
{
    [TestClass]
    public class VisionTestResultsMapperTests
    {
        private IFixture _fixture;
        private VisionTestResultsMapper _mapper;
        private ILogger<VisionTestResultsMapper> _logger;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<VisionTestResultsMapper>>();
            _mapper = new VisionTestResultsMapper(_logger);
        }

        [TestMethod]
        public void Vision_Test_Results_Mapper_Returns_Cleaned_Html_When_Results_Present()
        {
            // Arrange
            var currentDirectory = Directory.GetCurrentDirectory();
            var testResultsHtml = File.ReadAllText($"{currentDirectory}/GpSystems/Suppliers/Vision/PatientRecord/TestData/TestResults/VariousTestResults.html");
            var expectedCleanMarkup = File.ReadAllText($"{currentDirectory}/GpSystems/Suppliers/Vision/PatientRecord/TestData/TestResults/CleanedTestResults.html");
            
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                 Record = testResultsHtml
            });

            // Assert
            Assert.IsTrue(mappedResponse != null);
            Assert.IsTrue(!string.IsNullOrEmpty(mappedResponse.RawHtml));
            Assert.IsTrue(string.CompareOrdinal(expectedCleanMarkup, mappedResponse.RawHtml) == 0);
        }

        [TestMethod]
        public void Vision_Test_Results_Mapper_Returns_No_Markup_When_Results_Empty()
        {
            // Arrange
            var currentDirectory = Directory.GetCurrentDirectory();
            var testResultsHtml = File.ReadAllText($"{currentDirectory}/GpSystems/Suppliers/Vision/PatientRecord/TestData/TestResults/NoTestResults.html");
            
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = testResultsHtml
            });
            
            // Assert
            Assert.IsTrue(mappedResponse != null);
            Assert.IsTrue(string.IsNullOrEmpty(mappedResponse.RawHtml));
        }
    }
}
