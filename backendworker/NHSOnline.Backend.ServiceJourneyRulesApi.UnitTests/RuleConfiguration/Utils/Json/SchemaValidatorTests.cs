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
        [DataRow("\"homeScreen\": { \"publicHealthNotifications\": [{ \"id\": \"1\", \"type\": \"callout\", \"urgency\": \"warning\", \"title\": \"foo\", \"body\": \"bar\" }] }",
            DisplayName = "Valid configuration:: publicHealthNotifications")]

        [DataRow("\"appointments\": { \"provider\": \"im1\" }", DisplayName = "Valid configuration: appointments provider = im1")]
        [DataRow("\"appointments\": { \"provider\": \"informatica\", \"informaticaUrl\": \"http://example.com\" }", DisplayName = "Valid configuration: appointments provider = informatica, informaticaUrl = http://example.com")]
        [DataRow("\"appointments\": { \"provider\": \"gpAtHand\" }", DisplayName = "Valid configuration: appointments provider = gpAtHand")]
        [DataRow("\"cdssAdvice\": { \"provider\": \"none\" }", DisplayName = "Valid configuration: cdssAdmin provider = none")]
        [DataRow("\"cdssAdvice\": { \"provider\": \"eConsult\", \"serviceDefinition\": \"foo\", \"conditionsServiceDefinition\": \"bar\" }", DisplayName = "Valid configuration: cdssAdvice provider = eConsult")]
        [DataRow("\"cdssAdmin\": { \"provider\": \"none\" }", DisplayName = "Valid configuration: cdssAdmin provider = none")]
        [DataRow("\"cdssAdmin\": { \"provider\": \"eConsult\", \"serviceDefinition\": \"foo\" }", DisplayName = "Valid configuration: cdssAdmin provider = eConsult")]
        [DataRow("\"medicalRecord\": { \"provider\": \"im1\", \"version\": \"2\"}", DisplayName = "Valid configuration: medicalRecord provider = im1, version = 2")]
        [DataRow("\"medicalRecord\": { \"provider\": \"gpAtHand\", \"version\": \"2\"}", DisplayName = "Valid medicalRecord gpAtHand provider version 2 journey")]
        [DataRow("\"prescriptions\": { \"provider\": \"im1\"}", DisplayName = "Valid configuration: medicalRecord provider = im1")]
        [DataRow("\"prescriptions\": { \"provider\": \"gpAtHand\"}", DisplayName = "Valid configuration: medicalRecord provider = gpAtHand")]
        [DataRow("\"nominatedPharmacy\": \"true\"", DisplayName = "Valid configuration: nominatedPharmacy = true")]
        [DataRow("\"nominatedPharmacy\": \"false\"", DisplayName = "Valid configuration: nominatedPharmacy = false")]
        [DataRow("\"notifications\": \"true\"", DisplayName = "Valid configuration: notifications = true")]
        [DataRow("\"notifications\": \"false\"", DisplayName = "Valid configuration: notifications = false")]
        [DataRow("\"messaging\": \"true\"", DisplayName = "Valid configuration: messaging = true")]
        [DataRow("\"messaging\": \"false\"", DisplayName = "Valid configuration: messaging = false")]
        [DataRow("\"userInfo\": \"true\"", DisplayName = "Valid configuration: userInfo = true")]
        [DataRow("\"userInfo\": \"false\"", DisplayName = "Valid configuration: userInfo = false")]
        [DataRow("\"documents\": \"true\"", DisplayName = "Valid configuration: documents = true")]
        [DataRow("\"documents\": \"false\"", DisplayName = "Valid configuration:. documents = false")]
        [DataRow("\"supportsLinkedProfiles\": \"true\"", DisplayName = "Valid configuration: supportsLinkedProfiles = true")]
        [DataRow("\"supportsLinkedProfiles\": \"false\"", DisplayName = "Valid configuration:. supportsLinkedProfiles = false")]

        [DataRow("\"im1Messaging\": { \"isEnabled\": \"true\", \"canDeleteMessages\": \"true\", \"canUpdateReadStatus\": \"true\", \"requiresDetailsRequest\": \"true\", \"sendMessageSubject\": \"true\"}",
            DisplayName = "Valid all im1messaging implementations enabled journey")]

        [DataRow("\"im1Messaging\": { \"isEnabled\": \"false\", \"canDeleteMessages\": \"false\", \"canUpdateReadStatus\": \"false\", \"requiresDetailsRequest\": \"false\", \"sendMessageSubject\": \"false\"}",
            DisplayName = "Valid all im1messaging implementations disabled journey")]

        [DataRow("\"im1Messaging\": { \"isEnabled\": \"true\", \"canDeleteMessages\": \"false\", \"canUpdateReadStatus\": \"false\", \"requiresDetailsRequest\": \"false\", \"sendMessageSubject\": \"false\"}",
            DisplayName = "Valid configuration: im1messaging: isEnabled = true, canDeleteMessages = false, canUpdateReadStatus = false, requiresDetailRequest = false, sendMessageSubject = false")]

        [DataRow("\"im1Messaging\": { \"isEnabled\": \"true\", \"canDeleteMessages\": \"false\", \"canUpdateReadStatus\": \"true\", \"requiresDetailsRequest\": \"true\", \"sendMessageSubject\": \"true\"}",
            DisplayName = "Valid configuration: im1messaging: isEnabled = true, canDeleteMessages = false, canUpdateReadStatus = false, requiresDetailRequest = false, sendMessageSubject = false")]

        [DataRow("\"im1Messaging\": { \"isEnabled\": \"true\", \"canDeleteMessages\": \"true\", \"canUpdateReadStatus\": \"false\", \"requiresDetailsRequest\": \"false\", \"sendMessageSubject\": \"true\"}",
            DisplayName = "Valid configuration: im1messaging: isEnabled = true, canDeleteMessages = true, canUpdateReadStatus = false, requiresDetailRequest = false, sendMessageSubject = true")]
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
        [DataRow("homeScreen", "{}", "PropertyRequired: #/journeys.homeScreen.publicHealthNotifications", false)]
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
        [DataRow("im1Messaging", "{\"isEnabled\": \"true\"}", "PropertyRequired: #/journeys.im1Messaging.canDeleteMessages", true)]
        [DataRow("im1Messaging", "{\"isEnabled\": \"yes\", \"canDeleteMessages\" : \"true\"}", "NotInEnumeration: #/journeys.im1Messaging.isEnabled", false)]
        [DataRow("im1Messaging", "{\"isEnabled\": \"true\"}", "PropertyRequired: #/journeys.im1Messaging.canUpdateReadStatus", true)]
        [DataRow("im1Messaging", "{\"isEnabled\": \"yes\", \"canUpdateReadStatus\" : \"true\"}", "NotInEnumeration: #/journeys.im1Messaging.isEnabled", false)]
        [DataRow("im1Messaging", "{\"isEnabled\": \"true\"}", "PropertyRequired: #/journeys.im1Messaging.requiresDetailsRequest", true)]
        [DataRow("im1Messaging", "{\"isEnabled\": \"yes\", \"requiresDetailsRequest\" : \"true\"}", "NotInEnumeration: #/journeys.im1Messaging.isEnabled", false)]
        [DataRow("im1Messaging", "{\"isEnabled\": \"true\"}", "PropertyRequired: #/journeys.im1Messaging.sendMessageSubject", true)]
        [DataRow("supportsLinkedProfiles", "\"\"", "NotInEnumeration: #/journeys.supportsLinkedProfiles", false)]
        [DataRow("supportsLinkedProfiles", "\"test\"", "NotInEnumeration: #/journeys.supportsLinkedProfiles", false)]
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

        [DataTestMethod]
        [DataRow("undefined", "\"callout\"", "\"warning\"", "\"title\"", "\"body\"", "StringExpected: #/journeys.homeScreen.publicHealthNotifications[0].id")]
        [DataRow("1", "\"testType\"", "\"warning\"", "\"title\"", "\"body\"", "NotInEnumeration: #/journeys.homeScreen.publicHealthNotifications[0].type")]
        [DataRow("1", "\"callout\"", "\"testUrgency\"", "\"title\"", "\"body\"", "NotInEnumeration: #/journeys.homeScreen.publicHealthNotifications[0].urgency")]
        [DataRow("1", "\"callout\"", "\"warning\"", "undefined", "\"body\"", "StringExpected: #/journeys.homeScreen.publicHealthNotifications[0].title")]
        [DataRow("1", "\"callout\"", "\"warning\"", "\"title\"", "undefined", "StringExpected: #/journeys.homeScreen.publicHealthNotifications[0].body")]
        public async Task
            ValidateJsonAgainstSchema_JourneyConfigurationHomeScreen_WhenCalledWithInvalidJourney_ReturnsFalse(
                string id, string type, string urgency, string title, string body, string expectedError)
        {
            // Arrange
            var fileContent = "{" +
                              "  \"$schema\": \"Schemas/Journeys/configuration_schema.json\"," +
                              "  \"target\": {" +
                              "    \"odsCode\":\"FOO\"" +
                              "  }," +
                              "  \"journeys\": {" +
                              "    \"homeScreen\": {" +
                              "      \"publicHealthNotifications\": [{" +
                              "        \"id\": " + id + "," +
                              "        \"type\": " + type + "," +
                              "        \"urgency\": " + urgency + "," +
                              "        \"title\": " + title + "," +
                              "        \"body\": " + body +
                              "      }]" +
                              "    }" +
                              "  }" +
                              "}";
            var jsonFile = new FileData(string.Empty, fileContent);

            // Act
            var result = await _validator.ValidateJsonAgainstSchema(_journeyConfigurationSchema, jsonFile);

            // Assert
            _mockLogger.VerifyLogger(LogLevel.Error, expectedError, Times.Once());

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