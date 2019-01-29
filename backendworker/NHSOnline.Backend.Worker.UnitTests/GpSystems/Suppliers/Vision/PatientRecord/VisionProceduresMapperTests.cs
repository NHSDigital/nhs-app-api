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
    public class VisionProceduresMapperTests
    {
        private IFixture _fixture;
        private VisionProceduresMapper _mapper;
        private ILogger<VisionProceduresMapper> _logger;
        private Mock<IHtmlSanitizer> _htmlSanitizer;

        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<VisionProceduresMapper>>();
            _htmlSanitizer = new Mock<IHtmlSanitizer>(MockBehavior.Strict);
            _mapper = new VisionProceduresMapper(_logger, _htmlSanitizer.Object);
        }

        [TestMethod]
        public async Task Vision_Procedures_Mapper_Returns_Cleaned_Html_When_Results_Present()
        {
            //Arrange
            var proceduresHtml =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.Procedures.VariousProcedures.html");
            var expectedCleanMarkup =
                await EmbeddedResourceFileHelper.ReadEmbeddedResource(
                    "NHSOnline.Backend.Worker.UnitTests.GpSystems.Suppliers.Vision.PatientRecord.TestData.Procedures.CleanedProcedures.html");

            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns(proceduresHtml);

            //Act
            var mappedResponse = _mapper.Map(new VisionPatientDataResponse()
            {
                Record = proceduresHtml
            });

            //Assert
            Assert.IsTrue(mappedResponse != null);
            Assert.IsTrue(!string.IsNullOrEmpty(mappedResponse.RawHtml));

            Assert.IsTrue(string.CompareOrdinal(mappedResponse.RawHtml, expectedCleanMarkup) == 0);
        }
    }
}