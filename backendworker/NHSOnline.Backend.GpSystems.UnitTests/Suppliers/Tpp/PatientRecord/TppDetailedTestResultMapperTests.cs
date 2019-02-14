using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.GpSystems.Suppliers;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.Models.PatientRecord;
using NHSOnline.Backend.GpSystems.Suppliers.Tpp.PatientRecord;

namespace NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord
{
    [TestClass]
    public class TppDetailedTestResultMapperTests
    {
        private ITppDetailedTestResultMapper _mapper;
        private ILogger<TppDetailedTestResultMapper> _logger;
        private IFixture _fixture;
        private Mock<IHtmlSanitizer> _htmlSanitizer;
        
        [TestInitialize]
        public void TestInitialize()
        {
            _fixture = new Fixture().Customize(new AutoMoqCustomization());
            _logger = _fixture.Freeze<ILogger<TppDetailedTestResultMapper>>();
            _htmlSanitizer = new Mock<IHtmlSanitizer>(MockBehavior.Strict);
            _mapper = new TppDetailedTestResultMapper(_logger, _htmlSanitizer.Object);
        }

        [TestMethod]
        public async Task MapTestResultsViewReplyToTestResultsResponse_CallsHtmlSanitizer_WhenMappingHtml()
        {
            // Arrange
            var testResultsHtml =
                await ReadTestDataFile(
                    "NHSOnline.Backend.GpSystems.UnitTests.Suppliers.Tpp.PatientRecord.TestData.TppTestResultData.html");

            _htmlSanitizer.Setup(h => h.SanitizeHtml(It.IsAny<string>())).Returns("<p>sanitized html</p>");
            
            var testResultsViewReply = new TestResultsViewReply
            {
                Items = new List<TestResultsViewReplyItem>
                {
                    new TestResultsViewReplyItem
                    {
                        Value = testResultsHtml
                    }
                }
            };
            
            // Act
            var result = _mapper.Map(testResultsViewReply);
            
            // Assert
            _htmlSanitizer.Verify(mock => mock.SanitizeHtml(testResultsHtml));
            Assert.IsTrue(!string.IsNullOrEmpty(result.TestResult));
        }
        
        [TestMethod]
        public void MapTestResultsViewReplyToTestResultsResponse_WithEmptyValues_ReturnsResultWithEmptyValues()
        {
            // Arrange
            var item = new TestResultsViewReply();
            // Act
            var result = _mapper.Map(item);

            // Assert
            result.Should().NotBeNull();
            result.TestResult.Should().BeNull();           
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