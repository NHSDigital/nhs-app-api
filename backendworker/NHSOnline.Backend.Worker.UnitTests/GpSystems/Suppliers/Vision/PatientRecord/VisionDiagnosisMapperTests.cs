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
                await ReadTestDataFile(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.Diagnosis.VariousDiagnosis.html");
            var expectedCleanMarkup =
                await ReadTestDataFile(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.Diagnosis.CleanedDiagnosis.html");

            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(diagnosisHtml);
            
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