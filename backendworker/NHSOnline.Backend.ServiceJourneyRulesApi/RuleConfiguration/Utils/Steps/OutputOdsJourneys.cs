using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal class OutputOdsJourneys : IValidatorStep
    {
        public string Description { get; } = "Output Ods journeys";
        public ProcessOrder Order { get; } = ProcessOrder.OutputOdsJourneys;

        private readonly ILogger _logger;
        private readonly IYamlWriter _yamlWriter;
        private readonly string _outputFolder;

        public OutputOdsJourneys(
            ILogger<OutputOdsJourneys> logger,
            IYamlWriter yamlWriter,
            IServiceJourneyRulesConfiguration configuration)
        {
            _logger = logger;
            _yamlWriter = yamlWriter;
            _outputFolder = configuration.OutputFolderPath;
        }

        public Task<bool> Execute(ConfigurationContext context)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(context, nameof(context), ThrowError)
                .IsNotNull(context?.MergedOdsJourneys, nameof(context.MergedOdsJourneys), ThrowError)
                .IsValid();

            try
            {
                WriteContent(context.MergedOdsJourneys);
            }
            catch (Exception e)
            {
                _logger.LogCritical(e, "Error outputting the merged files. See output for specific errors.");
                return Task.FromResult(false);
            }

            return Task.FromResult(true);
        }

        private void WriteContent(IDictionary<string, Journeys> mergedOdsJourneys)
        {
            _logger.LogDebug($"Outputting merged ODS config files to {_outputFolder}");
            foreach (var (odsCode, journeys) in mergedOdsJourneys)
            {
                var targetConfiguration = new TargetConfiguration
                {
                    Target = new Target
                    {
                        OdsCode = odsCode
                    },
                    Journeys = journeys
                };

                var filePath = Path.Join(_outputFolder, $"{odsCode}.yaml");

                _logger.LogDebug($"Creating '{filePath}'");
                _yamlWriter.Write(filePath, targetConfiguration);
            }
        }
    }
}