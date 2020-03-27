using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord
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
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData.TestResults.VariousTestResults.html");
            
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = testResultsHtml
            });

            // Assert
            mappedResponse.Should().NotBeNull();
            _htmlSanitizer.Verify(mock => mock.SanitizeHtml(testResultsHtml));
        }
        
        [TestMethod]
        public async Task VisionTestResultsMapper_FormatsHtml_WhenMappingVisionTestResults()
        {
            // Arrange
            var sanitizedHtml =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData.TestResults.VariousTestResults.html");                              
            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(sanitizedHtml);
            
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = sanitizedHtml
            });

            // Assert
            mappedResponse.Should().NotBeNull();
            mappedResponse.RawHtml.Should().NotBeNullOrEmpty();
            mappedResponse.RawHtml.Should().Contain("tbody tr { line-height: 1.5em !important;");
        }
        
        [TestMethod]
        public async Task VisionTestResultsMapper_ReturnsNoMarkup_WhenResultsEmpty()
        {
            // Arrange
            var noTestResultsHtml =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData.TestResults.NoTestResults.html");          
            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(noTestResultsHtml);
        
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse
            {
                Record = noTestResultsHtml
            });
        
            // Assert
            mappedResponse.Should().NotBeNull();
            mappedResponse.RawHtml.Should().BeNullOrEmpty();
        }
    }
}