using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.UnitTests.RuleConfiguration.Utils
{
    [TestClass]
    public class ConfigurationRuleFileValidatorTests
    {
        private const string FileReadError = "error";
        private const string ValidateError = "validate_error";

        private const string RulesConfigFileName = "rules.yaml";
        private const string RulesConfigFileData = "rulesconfig: value";

        private const string JourneyConfigFileName = "journey_config.yaml";
        private const string JourneyConfigFileData = "journeyconfig: value";

        private const string RulesSchemaFileName = "rules_schema.json";
        private const string RulesSchemaFileData = "rules_schema: value";

        private const string JourneyConfigSchemaFileName = "configuration_schema.json";
        private const string JourneyConfigSchemaFileData = "configuration_schema: value";

        private Mock<IErrorHandler> _mockErrorHandler;
        private Mock<IFileHandler>_mockFileHandler;
        private Mock<IYamlToJsonConverter> _mockYamlToJsonConverter;
        private Mock<ISchemaValidator> _mockSchemaValidator;
        private ConfigurationRuleFileValidator _configurationRuleFileValidator;

        [TestInitialize]
        public void TestInitialize()
        {
            _mockErrorHandler = new Mock<IErrorHandler>();
            _mockFileHandler = new Mock<IFileHandler>();
            _mockYamlToJsonConverter = new Mock<IYamlToJsonConverter>();
            _mockSchemaValidator = new Mock<ISchemaValidator>();
            _configurationRuleFileValidator = new ConfigurationRuleFileValidator(_mockErrorHandler.Object,
                _mockFileHandler.Object, _mockYamlToJsonConverter.Object, _mockSchemaValidator.Object);
        }

        [TestMethod]
        [DataRow(false, true, true, true, 0)]
        [DataRow(true, false, true, true, 1)]
        [DataRow(true, true, false, true, 0)]
        [DataRow(true, true, true, false, 1)]
        [DataRow(true, false, true, false, 2)]
        public void ValidateJourneyConfigurationFiles_CalledWhenFilesMissing_LogsAppropriateMessages(
            bool rulesConfigExists, bool rulesSchemaExists, bool journeyConfigsExist, bool journeyConfigsSchemaExists,
            int expectedErrorCalls)
        {
            var criticalCalls = 0;
            var errorCalls = 0;
            var converterCalls = 0;

            SetupFileHandlerForRulesConfig(rulesConfigExists);
            SetupFileHandlerForRulesSchema(rulesSchemaExists);
            SetupFileHandlerForJourneyConfigs(journeyConfigsExist);
            SetupFileHandlerForJourneyConfigsSchema(journeyConfigsSchemaExists);

            _mockErrorHandler.Setup(errorHandler => errorHandler.LogError(FileReadError))
                .Callback(() => errorCalls++);
            _mockErrorHandler.Setup(errorHandler => errorHandler.LogCritical(It.IsAny<string>()))
                .Callback(() => criticalCalls++);
            _mockYamlToJsonConverter.Setup(converter => converter.Convert(It.IsAny<FileData>()))
                .Callback(() => converterCalls++);

            _configurationRuleFileValidator.ValidateJourneyConfigurationFiles();

            Assert.AreEqual(1, criticalCalls);
            Assert.AreEqual(expectedErrorCalls, errorCalls);
            Assert.AreEqual(0, converterCalls);
        }

        [TestMethod]
        public void ValidateJourneyConfigurationFiles_CalledWhenMultipleRulesFilesPresent_LogsAppropriateMessages()
        {
            var criticalCalls = 0;
            var errorCalls = 0;
            var converterCalls = 0;

            SetupFileHandlerForRulesConfig(true, true);
            SetupFileHandlerForRulesSchema();
            SetupFileHandlerForJourneyConfigs();
            SetupFileHandlerForJourneyConfigsSchema();

            _mockErrorHandler.Setup(errorHandler => errorHandler.LogError(It.IsAny<string>()))
                .Callback(() => errorCalls++);
            _mockErrorHandler.Setup(errorHandler => errorHandler.LogCritical(It.IsAny<string>()))
                .Callback(() => criticalCalls++);
            _mockYamlToJsonConverter.Setup(converter => converter.Convert(It.IsAny<FileData>()))
                .Callback(() => converterCalls++);

            _configurationRuleFileValidator.ValidateJourneyConfigurationFiles();

            Assert.AreEqual(1, criticalCalls);
            Assert.AreEqual(1, errorCalls);
            Assert.AreEqual(0, converterCalls);
        }

        [TestMethod]
        [DataRow(true, false, 1)]
        [DataRow(false, true, 1)]
        [DataRow(false, false, 2)]
        public void
            ValidateJourneyConfigurationFiles_CalledWhenAllFilesFoundButNotAllValidYaml_LogsAppropriateMessage(
                bool convertRulesSuccess, bool convertJourneyConfigsSuccess, int expectedErrorCalls)
        {
            var criticalCalls = 0;
            var errorCalls = 0;

            SetupFileHandlerForRulesConfig();
            SetupFileHandlerForRulesSchema();
            SetupFileHandlerForJourneyConfigs();
            SetupFileHandlerForJourneyConfigsSchema();

            SetupConverterToReturnConvertedFile(convertRulesSuccess);
            SetupConverterToReturnConvertedFiles(convertJourneyConfigsSuccess);

            _mockErrorHandler.Setup(errorHandler => errorHandler.LogError(FileReadError))
                .Callback(() => errorCalls++);
            _mockErrorHandler.Setup(errorHandler => errorHandler.LogCritical(It.IsAny<string>()))
                .Callback(() => criticalCalls++);

            _configurationRuleFileValidator.ValidateJourneyConfigurationFiles();

            Assert.AreEqual(1, criticalCalls);
            Assert.AreEqual(expectedErrorCalls, errorCalls);
        }

        [TestMethod]
        [DataRow(true, true, 0, 0)]
        [DataRow(false, true, 1, 1)]
        [DataRow(true, false, 1, 1)]
        [DataRow(false, false, 2, 1)]
        public void
            ValidateJourneyConfigurationFiles_CalledWhenYamlValid_LogsAppropriateMessageAndExitsIfNotAllMatchingSchema(
                bool rulesMatch, bool journeyConfigsMatch, int expectedErrorCount,
                int expectedCriticalCount)
        {
            var criticalCalls = 0;
            var errorCalls = 0;

            SetupFileHandlerForRulesConfig();
            SetupFileHandlerForRulesSchema();
            SetupFileHandlerForJourneyConfigs();
            SetupFileHandlerForJourneyConfigsSchema();

            SetupConverterToReturnConvertedFile();
            SetupConverterToReturnConvertedFiles();

            SetupSchemaValidator(rulesMatch, journeyConfigsMatch);

            _mockErrorHandler.Setup(errorHandler => errorHandler.LogError(ValidateError))
                .Callback(() => errorCalls++);
            _mockErrorHandler.Setup(errorHandler => errorHandler.LogCritical(It.IsAny<string>()))
                .Callback(() => criticalCalls++);

            _configurationRuleFileValidator.ValidateJourneyConfigurationFiles();

            Assert.AreEqual(expectedCriticalCount, criticalCalls);
            Assert.AreEqual(expectedErrorCount, errorCalls);
        }

        private void SetupFileHandlerForRulesConfig(bool exists = true, bool multiple = false)
        {
            var filesFound = new List<FileData>();

            if (exists)
            {
                var rulesConfigFile = CreateFileData(RulesConfigFileName, RulesConfigFileData);
                filesFound.Add(rulesConfigFile);

                if (multiple)
                {
                    filesFound.Add(rulesConfigFile);
                }
            }

            _mockFileHandler.Setup(erh => erh.ReadContentFilesFromDirectory(Constants.FolderNames.RulesConfiguration))
                .Returns(filesFound);
        }

        private void SetupFileHandlerForJourneyConfigs(bool filesExist = true)
        {
            var filesFound = new List<FileData>();

            if (filesExist)
            {
                filesFound.Add(CreateFileData(JourneyConfigFileName, JourneyConfigFileData));
            }

            _mockFileHandler.Setup(erh => erh.ReadContentFilesFromDirectory(Constants.FolderNames.JourneyConfigurations))
                .Returns(filesFound);
        }

        private void SetupFileHandlerForRulesSchema(bool exists = true)
        {
            _mockFileHandler.Setup(erh => erh.ReadEmbeddedResourceFromFileName(RulesSchemaFileName))
                .Returns(CreateFileData(RulesSchemaFileName, RulesSchemaFileData, !exists));
        }

        private void SetupFileHandlerForJourneyConfigsSchema(bool exists = true)
        {
            _mockFileHandler.Setup(erh => erh.ReadEmbeddedResourceFromFileName(JourneyConfigSchemaFileName))
                .Returns(CreateFileData(JourneyConfigSchemaFileName, JourneyConfigSchemaFileData, !exists));
        }

        private void SetupConverterToReturnConvertedFile(bool convertSuccessful = true)
        {
            _mockYamlToJsonConverter.Setup(converter => converter.Convert(It.IsAny<FileData>()))
                .Returns(CreateFileData(RulesConfigFileName, RulesConfigFileData, !convertSuccessful));
        }

        private void SetupConverterToReturnConvertedFiles(bool convertSuccessful = true)
        {
            _mockYamlToJsonConverter.Setup(converter => converter.ConvertAll(It.IsAny<IEnumerable<FileData>>()))
                .Returns(new List<FileData>
                {
                    CreateFileData(JourneyConfigFileName, JourneyConfigFileData, !convertSuccessful)
                });
        }

        private void SetupSchemaValidator(bool rulesConfigMatch, bool journeyConfigsMatch)
        {
            _mockSchemaValidator.Setup(validator =>
                    validator.ValidateJsonAgainstSchema(
                        It.Is<FileData>(fileData =>
                            fileData.Name.Equals(RulesSchemaFileName, StringComparison.Ordinal)),
                        It.Is<FileData>(fileData =>
                            fileData.Name.Equals(RulesConfigFileName, StringComparison.Ordinal))))
                .ReturnsAsync(CreateValidationResult(rulesConfigMatch));

            _mockSchemaValidator.Setup(validator =>
                    validator.ValidateJsonAgainstSchema(
                        It.Is<FileData>(fileData =>
                            fileData.Name.Equals(JourneyConfigSchemaFileName, StringComparison.Ordinal)),
                        It.Is<FileData>(fileData =>
                            fileData.Name.Equals(JourneyConfigFileName, StringComparison.Ordinal))))
                .ReturnsAsync(CreateValidationResult(journeyConfigsMatch));
        }

        private static JsonValidationResult CreateValidationResult(bool success)
        {
            var result = new JsonValidationResult
            {
                IsErrors = !success,
                Json = success ? "" : null
            };

            if (!success)
            {
                result.Errors.Add(ValidateError);
            }

            return result;
        }

        private static FileData CreateFileData(string name, string data = null, bool errored = false)
        {
            return new FileData
            {
                Name = name,
                Data = data,
                IsError = errored,
                Error = errored ? FileReadError : null
            };
        }
    }
}