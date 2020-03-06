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
            _mockLogger.VerifyLogger(LogLevel.Error, "NoAdditionalPropertiesAllowed: #/additionalProperty",
                Times.Once());

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
        [DataRow("\"folderOrder\": \"notAList\"", "ArrayExpected: #/folderOrder")]
        [DataRow("\"folderOrder\": { \"prop\": \"foo\" }", "ArrayExpected: #/folderOrder")]
        [DataRow("\"folderOrder\": []", "TooFewItems: #/folderOrder")]
        public async Task ValidateJsonAgainstSchema_RulesSchema_WhenCalledWithInvalidFolderOrderType_ReturnsFalse(
            string folderOrder, string expectedError)
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
            _mockLogger.VerifyLogger(LogLevel.Error, expectedError, Times.Once());

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
        [DataRow("\"odsCodes\":\"NotAList\"", "ArrayExpected: #/target.odsCodes")]
        [DataRow("\"odsCodes\": []", "TooFewItems: #/target.odsCodes")]
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
        [DataRow("\"odsCodes\":[\"foo\", \"bop\"]")]
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
        [DataRow("\"appointments\": { \"provider\": \"gpAtHand\" }")]
        [DataRow("\"cdssAdvice\": { \"provider\": \"none\" }")]
        [DataRow("\"cdssAdvice\": { \"provider\": \"eConsult\", \"serviceDefinition\": \"foo\", \"conditionsServiceDefinition\": \"bar\" }")]
        [DataRow("\"cdssAdmin\": { \"provider\": \"none\" }")]
        [DataRow("\"cdssAdmin\": { \"provider\": \"eConsult\", \"serviceDefinition\": \"foo\" }")]
        [DataRow("\"medicalRecord\": { \"provider\": \"im1\", \"version\": \"2\"}")]
        [DataRow("\"medicalRecord\": { \"provider\": \"gpAtHand\", \"version\": \"2\"}")]
        [DataRow("\"prescriptions\": { \"provider\": \"im1\"}")]
        [DataRow("\"prescriptions\": { \"provider\": \"gpAtHand\"}")]
        [DataRow("\"nominatedPharmacy\": \"true\"")]
        [DataRow("\"nominatedPharmacy\": \"false\"")]
        [DataRow("\"notifications\": \"true\"")]
        [DataRow("\"notifications\": \"false\"")]
        [DataRow("\"messaging\": \"true\"")]
        [DataRow("\"messaging\": \"false\"")]
        [DataRow("\"userInfo\": \"true\"")]
        [DataRow("\"userInfo\": \"false\"")]
        [DataRow("\"documents\": \"true\"")]
        [DataRow("\"documents\": \"false\"")]
        [DataRow("\"im1Messaging\": \"true\"")]
        [DataRow("\"im1Messaging\": \"false\"")]
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
        [DataRow("cdssAdvice", "{}", "PropertyRequired: #/journeys.cdssAdvice.provider", true)]
        [DataRow("cdssAdvice", "{\"provider\": \"foo\"}", "NotInEnumeration: #/journeys.cdssAdvice.provider", true)]
        [DataRow("cdssAdvice", "{\"provider\": \"eConsult\"}","PropertyRequired: #/journeys.cdssAdvice.serviceDefinition", true)]
        [DataRow("cdssAdvice", "{\"provider\": \"eConsult\", \"serviceDefinition\": \"foo\"}","PropertyRequired: #/journeys.cdssAdvice.conditionsServiceDefinition", true)]
        [DataRow("cdssAdmin", "{}", "PropertyRequired: #/journeys.cdssAdmin.provider", true)]
        [DataRow("cdssAdmin", "{\"provider\": \"foo\"}", "NotInEnumeration: #/journeys.cdssAdmin.provider", true)]
        [DataRow("cdssAdmin", "{\"provider\": \"eConsult\"}","PropertyRequired: #/journeys.cdssAdmin.serviceDefinition", true)]
        [DataRow("appointments", "{}", "PropertyRequired: #/journeys.appointments.provider", true)]
        [DataRow("appointments", "{\"provider\": \"foo\"}", "NotInEnumeration: #/journeys.appointments.provider", true)]
        [DataRow("appointments", "{\"provider\": \"informatica\"}","PropertyRequired: #/journeys.appointments.informaticaUrl", true)]
        [DataRow("medicalRecord", "{}", "PropertyRequired: #/journeys.medicalRecord.provider", false)]
        [DataRow("medicalRecord", "{\"provider\": \"foo\"}", "NotInEnumeration: #/journeys.medicalRecord.provider",false)]
        [DataRow("prescriptions", "{}", "PropertyRequired: #/journeys.prescriptions.provider", false)]
        [DataRow("prescriptions", "{\"provider\": \"foo\"}", "NotInEnumeration: #/journeys.prescriptions.provider",false)]
        [DataRow("nominatedPharmacy", "\"\"", "NotInEnumeration: #/journeys.nominatedPharmacy", false)]
        [DataRow("nominatedPharmacy", "\"test\"", "NotInEnumeration: #/journeys.nominatedPharmacy", false)]
        [DataRow("notifications", "\"\"", "NotInEnumeration: #/journeys.notifications", false)]
        [DataRow("notifications", "\"test\"", "NotInEnumeration: #/journeys.notifications", false)]
        [DataRow("messaging", "\"\"", "NotInEnumeration: #/journeys.messaging", false)]
        [DataRow("messaging", "\"test\"", "NotInEnumeration: #/journeys.messaging", false)]
        [DataRow("userInfo", "\"\"", "NotInEnumeration: #/journeys.userInfo", false)]
        [DataRow("userInfo", "\"test\"", "NotInEnumeration: #/journeys.userInfo", false)]
        [DataRow("documents", "\"\"", "NotInEnumeration: #/journeys.documents", false)]
        [DataRow("documents", "\"test\"", "NotInEnumeration: #/journeys.documents", false)]
        [DataRow("im1Messaging", "\"\"", "NotInEnumeration: #/journeys.im1Messaging", false)]
        [DataRow("im1Messaging", "\"test\"", "NotInEnumeration: #/journeys.im1Messaging", false)]
        public async Task
            ValidateJsonAgainstSchema_JourneyConfiguration_WhenCalledWithInvalidJourney_ReturnsFalse(
                string journeyType, string value, string expectedError, bool oneOfMultipleOptions)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              "    \"odsCode\":\"FOO\"" +
                              "  }," +
                              "  \"journeys\": {" +
                              "    \"" + journeyType + "\": " + value +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            if (oneOfMultipleOptions)
            {
                _mockLogger.VerifyLogger(LogLevel.Error, $"NotOneOf: #/journeys.{journeyType}", Times.Once());
            }

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