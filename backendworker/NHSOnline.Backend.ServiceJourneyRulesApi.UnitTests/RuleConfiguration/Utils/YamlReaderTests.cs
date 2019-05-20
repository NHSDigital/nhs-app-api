using System;
using System.IO;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;
using UnitTestHelper;
using YamlDotNet.Core;
using YamlDotNet.Serialization;
using YamlDotNet.Serialization.NamingConventions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    
    [TestClass]
    public class YamlReaderTests
    {
        private const string FilePath = "c:/foo.yaml";
        private const string FileContent = "$schema: \"Schemas/Journeys/configuration_schema.json\"\n" +
                                           "target:\n"+
                                           "  \"*\": \"*\"\n"+
                                           "journeys:\n"+
                                           "  appointments:\n"+
                                           "    journeyType: im1Appointments";

        private Mock<IYamlToJsonConverter> _mockYamlToJsonConverter;
        private Mock<ISchemaValidator> _mockSchemaValidator;
        private Mock<ILogger> _mockLogger;
        private Mock<IFileHandler> _mockFileHandler;
        
        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization()); 
            
            _mockYamlToJsonConverter = fixture.Freeze<Mock<IYamlToJsonConverter>>();
            _mockSchemaValidator = fixture.Freeze<Mock<ISchemaValidator>>();
            _mockLogger = fixture.Freeze<Mock<ILogger>>();
            _mockFileHandler = fixture.Freeze<Mock<IFileHandler>>();
        }

        [TestMethod]
        public void GetData_WhenTheFileDoesNotExist_ShouldThrowAnException()
        {
            // Arrange
            _mockFileHandler.Setup(f => f.GetTextReader(FilePath)).Throws<FileNotFoundException>();
            var yamlReader = SetupYamlReader<TargetConfiguration>(new FileData(FilePath, string.Empty));
            
            // Act and Assert
            Func<Task> act = async () => await yamlReader.GetData();
            act.Should().Throw<FileNotFoundException>();
        }

        [TestMethod]
        public async Task GetData_WhenThereIsNoSchema_ReturnNull()
        {
            
            // Arrange
            var yamlReader = SetupYamlReader<TargetConfiguration>();
            _mockFileHandler.Setup(f => f.GetTextReader(FilePath))
                .Returns(new StringReader(FileContent));
            
            // Act
            var result = await yamlReader.GetData();
            
            // Assert
            _mockFileHandler.VerifyNoOtherCalls();
            _mockYamlToJsonConverter.VerifyNoOtherCalls();
            _mockSchemaValidator.VerifyNoOtherCalls();
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetData_WhenFileContentIsInvalidYaml_And_SchemaIsProvided_ReturnNull()
        {
            // Arrange
            var schemaData = new FileData("c:/schema.json", string.Empty);
            var yamlReader = SetupYamlReader<TargetConfiguration>(schemaData);
            FileData convertedJson = null;
            
            _mockFileHandler.Setup(f => f.GetTextReader(FilePath))
                .Returns(new StringReader("invalid yaml file"));
            _mockYamlToJsonConverter.Setup(c => c.Convert(It.IsAny<FileData>(), out convertedJson))
                .Returns(false);
            _mockSchemaValidator.Setup(s => s.ValidateJsonAgainstSchema(schemaData, It.IsAny<FileData>()))
                .ReturnsAsync(true);
            
            // Act
            var result = await yamlReader.GetData();
            
            // Assert
            _mockFileHandler.Verify(f => f.GetTextReader(FilePath));
            _mockYamlToJsonConverter.Verify(c => c.Convert(It.IsAny<FileData>(), out convertedJson));
            _mockSchemaValidator.VerifyNoOtherCalls();
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeNull();
        }

        [TestMethod]
        public async Task GetData_WhenFileContentDoesNotMatchSchema_ReturnNull()
        {
            // Arrange
            var schemaData = new FileData("c:/schema.json", string.Empty);
            var yamlReader = SetupYamlReader<TargetConfiguration>(schemaData);
            var convertedJson = new FileData(null, null);
            
            _mockFileHandler.Setup(f => f.GetTextReader(FilePath))
                .Returns(new StringReader(FileContent));
            _mockYamlToJsonConverter.Setup(c => c.Convert(It.IsAny<FileData>(), out convertedJson))
                .Returns(true);
            _mockSchemaValidator.Setup(s => s.ValidateJsonAgainstSchema(schemaData, It.IsAny<FileData>()))
                .ReturnsAsync(false);
            
            // Act
            var result = await yamlReader.GetData();
            
            // Assert
            _mockFileHandler.Verify(f => f.GetTextReader(FilePath));
            _mockYamlToJsonConverter.Verify(c => c.Convert(It.IsAny<FileData>(), out convertedJson));
            _mockSchemaValidator.Verify(s => s.ValidateJsonAgainstSchema(It.IsAny<FileData>(), It.IsAny<FileData>()));
            _mockLogger.VerifyNoOtherCalls();

            result.Should().BeNull();
        }
        
        [TestMethod]
        public async Task GetData_WhenFileContentDoesNotMatchModel_ReturnNull()
        {
            // Arrange
            var schemaData = new FileData("c:/schema.json", string.Empty);
            var yamlReader = SetupYamlReader<TargetConfiguration>(schemaData);
            var convertedJson = new FileData(null, null);
            
            _mockFileHandler.Setup(f => f.GetTextReader(FilePath))
                .Returns(new StringReader("invalid file content"));
            _mockYamlToJsonConverter.Setup(c => c.Convert(It.IsAny<FileData>(), out convertedJson))
                .Returns(true);
            _mockSchemaValidator.Setup(s => s.ValidateJsonAgainstSchema(schemaData, It.IsAny<FileData>()))
                .ReturnsAsync(true);
            
            // Act
            var result = await yamlReader.GetData();
            
            // Assert
            _mockFileHandler.Verify(f => f.GetTextReader(FilePath));
            _mockYamlToJsonConverter.Verify(c => c.Convert(It.IsAny<FileData>(), out convertedJson));
            _mockSchemaValidator.Verify(s => s.ValidateJsonAgainstSchema(It.IsAny<FileData>(), It.IsAny<FileData>()));
            _mockLogger.VerifyLogger(LogLevel.Error,typeof(YamlException), Times.Once());

            result.Should().BeNull();
        }
        
        [TestMethod]
        public async Task GetData_WithValidFileContent_ReturnModel()
        {
            // Arrange
            var expectedFileModel = new TargetConfiguration
            {
                Schema = "Schemas/Journeys/configuration_schema.json",
                Target = new Target { All = "*" },
                Journeys = new Journeys
                {
                    Appointments = new Appointments { JourneyType = AppointmentsJourneyType.Im1Appointments }
                }
            };
            
            var schemaData = new FileData("c:/schema.json", string.Empty);
            var yamlReader = SetupYamlReader<TargetConfiguration>(schemaData);
            var convertedJson = new FileData(null, null);
            
            _mockFileHandler.Setup(f => f.GetTextReader(FilePath))
                .Returns(new StringReader(FileContent));
            _mockYamlToJsonConverter.Setup(c => c.Convert(It.IsAny<FileData>(), out convertedJson))
                .Returns(true);
            _mockSchemaValidator.Setup(s => s.ValidateJsonAgainstSchema(schemaData, It.IsAny<FileData>()))
                .ReturnsAsync(true);
            
            // Act
            var result = await yamlReader.GetData();
            
            // Assert
            _mockFileHandler.Verify(f => f.GetTextReader(FilePath));
            _mockYamlToJsonConverter.Verify(c => c.Convert(It.IsAny<FileData>(), out convertedJson));
            _mockSchemaValidator.Verify(s => s.ValidateJsonAgainstSchema(It.IsAny<FileData>(), It.IsAny<FileData>()));
            _mockLogger.VerifyNoOtherCalls();
            
            result.Should().NotBeNull().And.BeEquivalentTo(expectedFileModel);
        }
        
        [TestMethod]
        public async Task GetData_WhenCalledTwice_ReturnCachedModel()
        {
            // Arrange
            var schemaData = new FileData("c:/schema.json", string.Empty);
            var yamlReader = SetupYamlReader<TargetConfiguration>(schemaData);
            var convertedJson = new FileData(null, null);
            
            _mockFileHandler.Setup(f => f.GetTextReader(FilePath))
                .Returns(new StringReader(FileContent));
            _mockYamlToJsonConverter.Setup(c => c.Convert(It.IsAny<FileData>(), out convertedJson))
                .Returns(true);
            _mockSchemaValidator.Setup(s => s.ValidateJsonAgainstSchema(schemaData, It.IsAny<FileData>()))
                .ReturnsAsync(true);
            
            // Act
            var result1 = await yamlReader.GetData();
            var result2 = await yamlReader.GetData();
            
            // Assert
            result1.Should().NotBeNull().And.BeSameAs(result2);
        }

        private IYamlReader<TModel> SetupYamlReader<TModel>(FileData schemaData = null)
            where TModel: class, new()
        {
           return new YamlReader<TModel>(
                FilePath, 
                schemaData,
                new DeserializerBuilder().WithNamingConvention(new CamelCaseNamingConvention()).Build(),
                _mockLogger.Object,
                _mockYamlToJsonConverter.Object,
                _mockSchemaValidator.Object,
                _mockFileHandler.Object);
        }
    }
}