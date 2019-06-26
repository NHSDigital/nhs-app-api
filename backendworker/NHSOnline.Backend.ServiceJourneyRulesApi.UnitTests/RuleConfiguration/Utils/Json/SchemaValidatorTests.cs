using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;
using AutoFixture.AutoMoq;
using FluentAssertions;
using Microsoft.Extensions.Logging;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;
using UnitTestHelper;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils.Json
{
    [TestClass]
    public class SchemaValidatorTests
    {
        private Mock<ILogger<SchemaValidator>> _mockLogger;
        private ISchemaValidator _validator;
        private FileData _rulesSchema;
        private FileData _journeyConfigurationSchema;

        [TestInitialize]
        public void TestInitialize()
        {
            var fixture = new Fixture()
                .Customize(new AutoMoqCustomization());

            _rulesSchema = GetEmbeddedResource(Constants.FileNames.RulesSchema);
            _journeyConfigurationSchema = GetEmbeddedResource(Constants.FileNames.JourneyConfigurationSchema);
            _mockLogger = fixture.Freeze<Mock<ILogger<SchemaValidator>>>();

            _validator = fixture.Create<SchemaValidator>();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_WhenCalledWithNullSchemaFile_ReturnsFalse()
        {
            // Arrange
            const string fileContent = "{" +
                                       "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                                       "  \"target\": {" +
                                       "    \"*\":\"*\"" +
                                       "  }," +
                                       "  \"journeys\": {" +
                                       "    \"appointments\": {" +
                                       "      \"provider\": \"im1\"" +
                                       "    }" +
                                       "  }" +
                                       "}";
            var jsonFile = new FileData(string.Empty, fileContent);
            var schemaFile = new FileData("Schema file", null);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(schemaFile, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, $"Unable to create schema from {schemaFile.Name}", Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_RulesSchema_WhenCalledWithAdditionalProperty_ReturnsFalse()
        {
            // Arrange
            const string fileContent = "{" +
                                       "  \"$schema\": \"Rules/Journeys/rules_schema.json\"," +
                                       "  \"folderOrder\": [" +
                                       "    \"defaults\"," +
                                       "    \"suppliers\"," +
                                       "    \"informatica\"," +
                                       "    \"incident_overrides\"" +
                                       "  ]," +
                                       "  \"additionalProperty\": [" +
                                       "    \"defaults\"," +
                                       "    \"suppliers\"," +
                                       "    \"informatica\"," +
                                       "    \"incident_overrides\"" +
                                       "  ]" +
                                       "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_rulesSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "NoAdditionalPropertiesAllowed: #/additionalProperty", Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_RulesSchema_WhenCalledWithoutFolderOrder_ReturnsFalse()
        {
            // Arrange
            const string fileContent = "{" +
                                       "  \"$schema\": \"Schemas/Rules/rules_schema.json\"" +
                                       "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_rulesSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "PropertyRequired: #/folderOrder", Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("\"folderOrder\": \"notAList\"")]
        [DataRow("\"folderOrder\": { \"prop\": \"foo\" }")]
        public async Task ValidateJsonAgainstSchema_RulesSchema_WhenCalledWithInvalidFolderOrderType_ReturnsFalse(
            string folderOrder)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Rules/rules_schema.json\"," +
                              folderOrder +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_rulesSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "ArrayExpected: #/folderOrder", Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_RulesSchema_WhenCalledWithValidJsonFile_ReturnsTrue()
        {
            // Arrange
            const string fileContent = "{" +
                                       "  \"$schema\": \"Schemas/Rules/rules_schema.json\"," +
                                       "  \"folderOrder\": [" +
                                       "    \"defaults\"," +
                                       "    \"suppliers\"," +
                                       "    \"informatica\"," +
                                       "    \"incident_overrides\"" +
                                       "  ]" +
                                       "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_rulesSchema, jsonFile);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithoutTarget_ReturnsFalse()
        {
            // Arrange
            const string fileContent = "{" +
                                       "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                                       "  \"journeys\": {" +
                                       "    \"appointments\": {" +
                                       "      \"provider\": \"im1\"" +
                                       "    }" +
                                       "  }" +
                                       "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "PropertyRequired: #/target", Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("", null)]
        [DataRow("\"*\":\"error\"", "NotInEnumeration: #/target.*")]
        [DataRow("\"supplier\":\"foo\"", "NotInEnumeration: #/target.supplier")]
        public async Task
            ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithInvalidTarget_ReturnsFalse(string target,
                string expectedError)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              target +
                              "  }," +
                              "  \"journeys\": {" +
                              "    \"appointments\": {" +
                              "      \"provider\": \"im1\"" +
                              "    }" +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert

            _mockLogger.VerifyLogger(LogLevel.Error, "NotOneOf: #/target", Times.Once());
            if (!string.IsNullOrWhiteSpace(expectedError))
            {
                _mockLogger.VerifyLogger(LogLevel.Error, expectedError, Times.Once());
            }

            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("\"supplier\":\"emis\"")]
        [DataRow("\"supplier\":\"tpp\"")]
        [DataRow("\"supplier\":\"microtest\"")]
        [DataRow("\"supplier\":\"vision\"")]
        [DataRow("\"ccgCode\":\"foo\"")]
        [DataRow("\"odsCode\":\"foo\"")]
        [DataRow("\"*\":\"*\"")]
        public async Task ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithValidTarget_ReturnsTrue(
            string target)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              target +
                              "  }," +
                              "  \"journeys\": {" +
                              "    \"appointments\": {" +
                              "      \"provider\": \"im1\"" +
                              "    }" +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        public async Task ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithoutJourneys_ReturnsFalse()
        {
            // Arrange
            const string fileContent = "{" +
                                       "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                                       "  \"target\": {" +
                                       "    \"supplier\":\"emis\"" +
                                       "  }" +
                                       "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "PropertyRequired: #/journeys", Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("")]
        [DataRow("\"test\": \"foo\"")]
        public async Task ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithInvalidJourneys_ReturnsFalse(
            string journeys)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              "    \"supplier\":\"emis\"" +
                              "  }," +
                              "  \"journeys\": {" +
                              journeys +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "NotAnyOf: #/journeys", Times.Once());

            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("\"appointments\": { \"provider\": \"im1\" }")]
        [DataRow("\"appointments\": { \"provider\": \"informatica\", \"informaticaUrl\": \"http://example.com\" }")]
        [DataRow("\"cdssAdvice\": { \"provider\": \"none\" }")]
        [DataRow("\"cdssAdvice\": { \"provider\": \"eConsult\", \"serviceDefinition\": \"foo\" }")]
        [DataRow("\"cdssAdmin\": { \"provider\": \"none\" }")]
        [DataRow("\"cdssAdmin\": { \"provider\": \"eConsult\", \"serviceDefinition\": \"foo\" }")]
        public async Task ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithValidJourneys_ReturnsTrue(
            string journeys)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              "    \"supplier\":\"emis\"" +
                              "  }," +
                              "  \"journeys\": {" +
                              journeys +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            result.Should().BeTrue();
        }

        [TestMethod]
        [DataRow("", null)]
        [DataRow("\"provider\": \"foo\"", "NotInEnumeration: #/journeys.cdssAdvice.provider")]
        [DataRow("\"provider\": \"eConsult\"", "PropertyRequired: #/journeys.cdssAdvice.serviceDefinition")]
        public async Task
            ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithInvalidCdssAdviceProvider_ReturnsFalse(
                string provider, string expectedError)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              "    \"odsCode\":\"FOO\"" +
                              "  }," +
                              "  \"journeys\": {" +
                              "    \"cdssAdvice\": {" +
                              provider +
                              "    }" +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "NotOneOf: #/journeys.cdssAdvice", Times.Once());
            if (!string.IsNullOrWhiteSpace(expectedError))
            {
                _mockLogger.VerifyLogger(LogLevel.Error, expectedError, Times.Once());
            }

            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("", null)]
        [DataRow("\"provider\": \"foo\"", "NotInEnumeration: #/journeys.cdssAdmin.provider")]
        [DataRow("\"provider\": \"eConsult\"", "PropertyRequired: #/journeys.cdssAdmin.serviceDefinition")]
        public async Task
            ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithInvalidCdssAdminProvider_ReturnsFalse(
                string provider, string expectedError)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              "    \"odsCode\":\"FOO\"" +
                              "  }," +
                              "  \"journeys\": {" +
                              "    \"cdssAdmin\": {" +
                              provider +
                              "    }" +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "NotOneOf: #/journeys.cdssAdmin", Times.Once());
            if (!string.IsNullOrWhiteSpace(expectedError))
            {
                _mockLogger.VerifyLogger(LogLevel.Error, expectedError, Times.Once());
            }

            result.Should().BeFalse();
        }

        [TestMethod]
        [DataRow("", null)]
        [DataRow("\"provider\": \"foo\"", "NotInEnumeration: #/journeys.appointments.provider")]
        [DataRow("\"provider\": \"informatica\"", "PropertyRequired: #/journeys.appointments.informaticaUrl")]
        public async Task
            ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithInvalidAppointmentsProvider_ReturnsFalse(
                string provider, string expectedError)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              "    \"odsCode\":\"FOO\"" +
                              "  }," +
                              "  \"journeys\": {" +
                              "    \"appointments\": {" +
                              provider +
                              "    }" +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, "NotOneOf: #/journeys.appointments", Times.Once());
            if (!string.IsNullOrWhiteSpace(expectedError))
            {
                _mockLogger.VerifyLogger(LogLevel.Error, expectedError, Times.Once());
            }

            result.Should().BeFalse();
        }

        private static FileData GetEmbeddedResource(string fileName)
        {
            var assembly = Assembly.GetAssembly(typeof(SchemaValidator));
            var resourceName = assembly.GetManifestResourceNames()
                .Single(str => str.EndsWith(fileName, StringComparison.Ordinal));

            using (var resourceStream = assembly.GetManifestResourceStream(resourceName))
            using (var reader = new StreamReader(resourceStream, Encoding.UTF8))
            {
                return new FileData(resourceName, reader.ReadToEnd());
            }
        }
    }
}