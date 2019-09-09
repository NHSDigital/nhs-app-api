using System.IO;
using AutoFixture;
using AutoFixture.AutoMoq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class YamlWriterTests
    {
        private IYamlWriter _yamlWriter;
        private Mock<IYamlSerializer> _mockYamlSerializer;
        private Mock<IFileHandler> _mockFileHandler;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _mockYamlSerializer = fixture.Freeze<Mock<IYamlSerializer>>();
            _mockFileHandler = fixture.Freeze<Mock<IFileHandler>>();

            _yamlWriter = fixture.Create<YamlWriter>();
        }

        [TestMethod]
        public void Write_WhenGivenAFilePathAndContent_WriteFile()
        {
            // Arrange
            const string filePath = "c:/test.yaml";
            var model = new object();

            // Act
            _yamlWriter.Write(filePath, model);

            // Act and Assert
            _mockFileHandler.Verify(s => s.GetTextWriter(filePath));
            _mockYamlSerializer.Verify(s => s.Serialize(It.IsAny<TextWriter>(), model));
        }
    }
}