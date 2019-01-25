using System;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.Areas.MyRecord.Models;
using NHSOnline.Backend.Worker.GpSystems.Suppliers;
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
        private Mock<IHtmlSanitizer> _htmlSanitizer;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<VisionTestResultsMapper>>();
            _htmlSanitizer = new Mock<IHtmlSanitizer>(MockBehavior.Strict);
            _mapper = new VisionTestResultsMapper(_logger, _htmlSanitizer.Object);
        }

        [TestMethod]
        public async Task VisionTestResultsMapper_CallsHtmlSanitizer_WhenMappingVisionTestResults()
        {
            // Arrange
            var testResultsHtml =
                await ReadTestDataFile(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.TestResults.VariousTestResults.html");
            
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = testResultsHtml
            });

            // Assert
            Assert.IsTrue(mappedResponse != null);
            _htmlSanitizer.Verify(mock => mock.SanitizeHtml(testResultsHtml));
        }
        
        [TestMethod]
        public async Task VisionTestResultsMapper_FormatsHtml_WhenMappingVisionTestResults()
        {
            // Arrange
            var sanitizedHtml =
                await ReadTestDataFile(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.TestResults.VariousTestResults.html");                              
            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(sanitizedHtml);
            
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = sanitizedHtml
            });

            // Assert
            Assert.IsTrue(mappedResponse != null);
            Assert.IsTrue(!string.IsNullOrEmpty(mappedResponse.RawHtml));
            Assert.IsTrue(!mappedResponse.RawHtml.Contains("<h3", StringComparison.OrdinalIgnoreCase),
                "h3 tag should be replaced by h4");
            Assert.IsTrue(mappedResponse.RawHtml.Contains("<h4", StringComparison.OrdinalIgnoreCase),
                "h4 tag not found");
            Assert.IsTrue(mappedResponse.RawHtml.Contains("tbody tr { line-height: 1.5em !important;",
                    StringComparison.OrdinalIgnoreCase),
                "tbody tr line height not adjusted");
           
        }
        
        [TestMethod]
        public async Task VisionTestResultsMapper_ReturnsNoMarkup_WhenResultsEmpty()
        {
            // Arrange
            var noTestResultsHtml =
                await ReadTestDataFile(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.TestResults.NoTestResults.html");          
            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(noTestResultsHtml);
        
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = noTestResultsHtml
            });
        
            // Assert
            Assert.IsTrue(mappedResponse != null);
            Assert.IsTrue(string.IsNullOrEmpty(mappedResponse.RawHtml));
        }

        private static async Task<string> ReadTestDataFile(string resourceFile)
        {
            var assembly = Assembly.GetExecutingAssembly();
            var resourceStream = assembly.GetManifestResourceStream(resourceFile);

            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return await reader.ReadToEndAsync();
            }
        }
    }
}
