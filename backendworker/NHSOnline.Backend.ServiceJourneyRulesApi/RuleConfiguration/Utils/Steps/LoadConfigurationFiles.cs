using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal class LoadConfigurationFiles : IValidatorStep, ILoadStep
    {
        public string Description { get; } = "Loading configuration files and validate schema";
        public ProcessOrder Order { get; } = ProcessOrder.LoadConfigurationFiles;

        private readonly ILogger _logger;
        private readonly IYamlReaderFactory _yamlReaderFactory;
        private readonly IFileHandler _fileHandler;
        private readonly IServiceJourneyRulesConfiguration _serviceJourneyRulesConfiguration;

        public LoadConfigurationFiles(
            ILogger<LoadConfigurationFiles> logger,
            IYamlReaderFactory yamlReaderFactory,
            IFileHandler fileHandler,
            IServiceJourneyRulesConfiguration serviceJourneyRulesConfiguration)
        {
            _logger = logger;
            _yamlReaderFactory = yamlReaderFactory;
            _fileHandler = fileHandler;
            _serviceJourneyRulesConfiguration = serviceJourneyRulesConfiguration;
        }

        public async Task<bool> Execute(LoadContext context)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(context, nameof(context), ThrowError)
                .IsNotNull(context?.TargetSchema, nameof(context.TargetSchema), ThrowError)
                .IsValid();

            var configFolderPath = _serviceJourneyRulesConfiguration.InputFolderPath;

            var (empty, didError, readFiles) = await GetConfigurations(context.TargetSchema, configFolderPath);
            if (empty || didError || !Validate(readFiles))
            {
                _logger.LogCritical("Error reading target configuration files. See output above for specific errors.");
                return false;
            }

            context.MergedOdsJourneys = readFiles.ToDictionary(k => k.Target.OdsCode, v => v.Journeys);
            return true;
        }

        public async Task<bool> Execute(ConfigurationContext context)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(context, nameof(context), ThrowError)
                .IsNotNull(context?.RulesSchema, nameof(context.RulesSchema), ThrowError)
                .IsNotNull(context?.TargetSchema, nameof(context.TargetSchema), ThrowError)
                .IsValid();

            var rules = await GetRulesConfig(context.RulesSchema);
            if (rules == null)
            {
                _logger.LogCritical("Error reading rules config file. See output above for specific errors.");
                return false;
            }

            var (didError, folderConfigurations) = await GetFolderConfigurations(rules, context.TargetSchema);
            if (didError)
            {
                _logger.LogCritical("Error reading target configuration files. See output above for specific errors.");
                return false;
            }

            context.FolderConfigurations = folderConfigurations;
            return true;
        }

        private async Task<Rules> GetRulesConfig(FileData schema)
        {
            var files = _fileHandler.GetFiles(_serviceJourneyRulesConfiguration.RulesFolderPath);

            if (files.Length != 1)
            {
                _logger.LogError(
                    $"There must be exactly 1 rules configuration file in {_serviceJourneyRulesConfiguration.RulesFolderPath}");
                return null;
            }

            var reader = _yamlReaderFactory.GetReader<Rules>(files.First(), schema);
            return await reader.GetData();
        }

        private async Task<(bool hadError, IDictionary<string, IEnumerable<TargetConfiguration>> folderConfiguations)>
            GetFolderConfigurations(
                Rules rules,
                FileData schema)
        {
            var hasError = false;
            var result = new Dictionary<string, IEnumerable<TargetConfiguration>>();

            foreach (var folderPath in rules.FolderOrder)
            {
                var configFolderPath = Path.Join(_serviceJourneyRulesConfiguration.JourneysFolderPath, folderPath);
                var (empty, didError, readFiles) = await GetConfigurations(schema, configFolderPath);

                if (empty)
                {
                    continue;
                }

                hasError = hasError || didError;
                result.Add(configFolderPath, readFiles);
            }

            return (hasError || !result.Any(), result);
        }

        private async Task<(bool empty, bool hadError, List<TargetConfiguration> configurations)> GetConfigurations(
            FileData schema,
            string configFolderPath)
        {
            var files = _fileHandler.GetFiles(configFolderPath);

            if (files.Any())
            {
                var (didError, readFiles) = await GetConfigurations(schema, files);
                return (false, didError, readFiles);
            }

            _logger.LogWarning(
                $"No target configuration files found in directory {configFolderPath}");
            return (true, false, null);
        }

        private async Task<(bool hadError, List<TargetConfiguration> configurations)> GetConfigurations(
            FileData schema,
            IEnumerable<string> files)
        {
            var hasError = false;
            var readFiles = new List<TargetConfiguration>();
            foreach (var filePath in files)
            {
                var reader = _yamlReaderFactory.GetReader<TargetConfiguration>(filePath, schema);
                var targetConfiguration = await reader.GetData();

                if (targetConfiguration == null)
                {
                    hasError = true;
                    continue;
                }

                readFiles.Add(targetConfiguration);
            }

            return (hasError, readFiles);
        }

        private bool Validate(IEnumerable<TargetConfiguration> readFiles)
        {
            if (!readFiles.Any(x => string.IsNullOrWhiteSpace(x.Target?.OdsCode)))
            {
                return true;
            }

            _logger.LogCritical(
                "Ods code is empty or null in mounted configuration files. See output above for specific errors.");
            return false;
        }
    }
}