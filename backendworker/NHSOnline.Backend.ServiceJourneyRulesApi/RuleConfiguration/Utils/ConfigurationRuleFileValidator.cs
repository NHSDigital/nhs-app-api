using System.Collections.Generic;
using System.Linq;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Converters;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Json;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    public class ConfigurationRuleFileValidator
    {
        private readonly IErrorHandler _errorHandler;
        private readonly IFileHandler _fileHandler;
        private readonly IYamlToJsonConverter _yamlToJsonConverter;
        private readonly ISchemaValidator _schemaValidator;

        public ConfigurationRuleFileValidator(IErrorHandler errorHandler, IFileHandler fileHandler, IYamlToJsonConverter yamlToJsonConverter, ISchemaValidator schemaValidator)
        {
            _errorHandler = errorHandler;
            _fileHandler = fileHandler;
            _yamlToJsonConverter = yamlToJsonConverter;
            _schemaValidator = schemaValidator;
        }

        public int ValidateJourneyConfigurationFiles()
        {
            var rulesConfig = GetRulesConfig();
            var journeyConfigs = GetJourneyConfigs();
            var rulesSchema = GetRulesSchema();
            var journeyConfigSchema = GetConfigurationSchema();

            if (rulesConfig == null || journeyConfigs == null || rulesSchema == null || journeyConfigSchema == null)
            {
                _errorHandler.LogCritical("Error reading necessary files. See output above for specific errors.");
                return 1;
            }

            var rulesConfigAsJson = ConvertRulesConfig(rulesConfig);
            var journeyConfigsAsJson = ConvertJourneyConfigs(journeyConfigs);

            if (rulesConfigAsJson == null || journeyConfigsAsJson == null)
            {
                _errorHandler.LogCritical("Error converting Yaml files to JSON. See output above for specific errors.");
                return 1;
            }

            var rulesConfigValid = ValidateJsonFileAgainstSchema(rulesSchema, rulesConfigAsJson);
            var journeysConfigValid = ValidateJsonFilesAgainstSchema(journeyConfigSchema, journeyConfigsAsJson);

            if (rulesConfigValid && journeysConfigValid)
            {
                return 0;
            }

            _errorHandler.LogCritical("Error validating configuration files. See output above for specific errors.");
            return 1;

        }

        private FileData GetRulesConfig()
        {
            var rulesConfig = _fileHandler.ReadContentFilesFromDirectory(Constants.FolderNames.RulesConfiguration);

            if (rulesConfig.Count == 1)
            {
                return rulesConfig.First();
            }

            _errorHandler.LogError(
                $"Error reading rules config file. Must be exactly 1 configuration file in {Constants.FolderNames.RulesConfiguration}");
            return null;
        }

        private IEnumerable<FileData> GetJourneyConfigs()
        {
            var journeyConfigs = _fileHandler.ReadContentFilesFromDirectory(Constants.FolderNames.JourneyConfigurations);

            if (journeyConfigs.Count > 0)
            {
                return journeyConfigs;
            }

            _errorHandler.LogError(
                $"Error reading journey config files. None found in directory {Constants.FolderNames.JourneyConfigurations}");
            return null;

        }

        private FileData GetRulesSchema()
        {
            var rulesSchema = _fileHandler.ReadEmbeddedResourceFromFileName(Constants.FileNames.RulesSchema);

            if (!rulesSchema.IsError)
            {
                return rulesSchema;
            }

            _errorHandler.LogError(rulesSchema.Error);
            return null;
        }

        private FileData GetConfigurationSchema()
        {
            var configurationSchema =
                _fileHandler.ReadEmbeddedResourceFromFileName(Constants.FileNames.JourneyConfigurationSchema);

            if (!configurationSchema.IsError)
            {
                return configurationSchema;
            }

            _errorHandler.LogError(configurationSchema.Error);
            return null;
        }

        private FileData ConvertRulesConfig(FileData rulesConfig)
        {
            var rulesConfigAsJson = _yamlToJsonConverter.Convert(rulesConfig);

            if (!rulesConfigAsJson.IsError)
            {
                return rulesConfigAsJson;
            }

            _errorHandler.LogError(rulesConfigAsJson.Error);
            return null;
        }

        private IEnumerable<FileData> ConvertJourneyConfigs(IEnumerable<FileData> journeyConfigs)
        {
            var journeyConfigsAsJson = _yamlToJsonConverter.ConvertAll(journeyConfigs).ToList();
            var journeyConfigsAsJsonWithError = journeyConfigsAsJson.Where(fileData =>
            {
                if (fileData.IsError)
                {
                    _errorHandler.LogError(fileData.Error);
                }

                return fileData.IsError;
            }).ToList();

            return journeyConfigsAsJsonWithError.Count > 0 ? null : journeyConfigsAsJson;
        }

        private bool ValidateJsonFileAgainstSchema(FileData schema, FileData json)
        {
            return ValidateJsonFilesAgainstSchema(schema, new List<FileData>{ json });
        }

        private bool ValidateJsonFilesAgainstSchema(FileData schemaFile, IEnumerable<FileData> jsonFiles)
        {
            var success = true;

            foreach (var jsonFile in jsonFiles)
            {
                var validationTask = _schemaValidator.ValidateJsonAgainstSchema(schemaFile, jsonFile);
                validationTask.Wait();

                var validationResult = validationTask.Result;

                if (!validationResult.IsErrors)
                {
                    continue;
                }

                success = false;
                foreach (var error in validationResult.Errors)
                {
                    _errorHandler.LogError(error);
                }
            }

            return success;
        }
    }
}