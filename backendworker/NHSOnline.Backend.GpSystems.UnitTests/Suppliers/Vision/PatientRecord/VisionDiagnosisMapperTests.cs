using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
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
        private IFixture _fixture;
        private VisionDiagnosisMapper _mapper;
        private ILogger<VisionDiagnosisMapper> _logger;
        private Mock<IHtmlSanitizer> _htmlSanitizer;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<VisionDiagnosisMapper>>();
            _htmlSanitizer = new Mock<IHtmlSanitizer>(MockBehavior.Strict);
            _mapper = new VisionDiagnosisMapper(_logger, _htmlSanitizer.Object);
        }

        [TestMethod]
        public async Task Vision_Diagnosis_Mapper_Returns_Cleaned_Html_When_Results_Present()
        {
            //Arrange
            var diagnosisHtml =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData.Diagnosis.VariousDiagnosis.html");
            var expectedCleanMarkup =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Vision.PatientRecord.TestData.Diagnosis.CleanedDiagnosis.html");

            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>(), null)).Returns(diagnosisHtml);
            
            //Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = diagnosisHtml
            });

            //Assert
            Assert.IsTrue(mappedResponse != null);
            Assert.IsTrue(!string.IsNullOrEmpty(mappedResponse.RawHtml));

            Assert.IsTrue(string.CompareOrdinal(mappedResponse.RawHtml, expectedCleanMarkup) == 0);
        }
    }
}