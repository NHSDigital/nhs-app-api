using System.Threading.Tasks;
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
    public class VisionDiagnosisMapperTests
    {
        private VisionDiagnosisMapper _mapper;
        private Mock<ILogger<VisionDiagnosisMapper>> _logger;
        private Mock<IHtmlSanitizer> _htmlSanitizer;

        [TestInitialize]
        public void TestInitialize()
        {
            _logger = new Mock<ILogger<VisionDiagnosisMapper>>();
            _htmlSanitizer = new Mock<IHtmlSanitizer>(MockBehavior.Strict);
            _mapper = new VisionDiagnosisMapper(_logger.Object, _htmlSanitizer.Object);
        }

        [TestMethod]
        public async Task Vision_Diagnosis_Mapper_Returns_Cleaned_Html_When_Results_Present()
        {
            // Arrange
            var diagnosisHtml =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData.Diagnosis.VariousDiagnosis.html");
            var expectedCleanMarkup =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData.Diagnosis.CleanedDiagnosis.html");

            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(diagnosisHtml);
            
            // Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = diagnosisHtml
            });

            // Assert
            mappedResponse.Should().NotBeNull();
            mappedResponse.RawHtml.Should().NotBeNullOrEmpty();
            mappedResponse.RawHtml.Should().Be(expectedCleanMarkup);
        }
    }
}