using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.Worker.GpSystems.Suppliers;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.Models.PatientRecord;
using NHSOnline.Backend.Worker.GpSystems.Suppliers.Vision.PatientRecord.ViewMapper;

namespace NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord
{
    [TestClass]
    public class VisionExaminationsMapperTests
    {
        private IFixture _fixture;
        private VisionExaminationsMapper _mapper;
        private ILogger<VisionExaminationsMapper> _logger;
        private Mock<IHtmlSanitizer> _htmlSanitizer;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<VisionExaminationsMapper>>();
            _htmlSanitizer = new Mock<IHtmlSanitizer>(MockBehavior.Strict);
            _mapper = new VisionExaminationsMapper(_logger, _htmlSanitizer.Object);
        }

        [TestMethod]
        public async Task Vision_Examinations_Mapper_Returns_Cleaned_Html_When_Results_Present()
        {
            //Arrange
            var examinationsHtml =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.Examinations.VariousExaminations.html");
            var expectedCleanMarkup =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.Examinations.CleanedExaminations.html");

            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(examinationsHtml);
            
            //Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = examinationsHtml
            });

            //Assert
            Assert.IsTrue(mappedResponse != null);
            Assert.IsTrue(!string.IsNullOrEmpty(mappedResponse.RawHtml));

            Assert.IsTrue(string.CompareOrdinal(mappedResponse.RawHtml, expectedCleanMarkup) == 0);
        }
    }
}