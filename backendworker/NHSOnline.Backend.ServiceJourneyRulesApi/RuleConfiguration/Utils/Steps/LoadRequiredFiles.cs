using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal class LoadRequiredFiles : IValidatorStep
    {
        public string Description { get; } = "Loading the required files";
        public ProcessOrder Order { get; } = ProcessOrder.LoadRequiredFiles;

        private readonly ILogger _logger;
        private readonly IFileHandler _fileHandler;
        private readonly IGpInfoReader _gpInfoReader;
        private readonly IServiceJourneyRulesConfiguration _serviceJourneyRulesConfiguration;

        public LoadRequiredFiles(
            ILogger<LoadRequiredFiles> logger,
            IFileHandler fileHandler,
            IGpInfoReader gpInfoReader,
            IServiceJourneyRulesConfiguration serviceJourneyRulesConfiguration)
        {
            _logger = logger;
            _fileHandler = fileHandler;
            _gpInfoReader = gpInfoReader;
            _serviceJourneyRulesConfiguration = serviceJourneyRulesConfiguration;
        }

        public Task<bool> Execute(ConfigurationContext context)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(context, nameof(context), ThrowError)
                .IsValid();

            context.RulesSchema = GetRulesSchema();
            context.TargetSchema = GetConfigurationSchema();
            context.GpInfos = _gpInfoReader.GetGpInfo(_serviceJourneyRulesConfiguration.GpInfoFilePath);

            if (context.RulesSchema != null && context.TargetSchema != null && context.GpInfos != null)
            {
                return Task.FromResult(true);
            }

            _logger.LogCritical("Error reading necessary files. See output above for specific errors.");
            return Task.FromResult(false);
        }

        private FileData GetRulesSchema() => GetEmbeddedData(Constants.FileNames.RulesSchema);

        private FileData GetConfigurationSchema() => GetEmbeddedData(Constants.FileNames.JourneyConfigurationSchema);

        private FileData GetEmbeddedData(string filePath) =>
            _fileHandler.ReadEmbeddedResourceFromFileName(filePath, out var data) ? data : null;
    }
}