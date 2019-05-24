using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    internal class ServiceJourneyRulesConfiguration : IServiceJourneyRulesConfiguration
    {
        public string GpInfoFilePath { get; }
        public string OutputFolderPath { get; }
        public string RulesFolderPath { get; }
        public string JourneysFolderPath { get; }
        public string InputFolderPath { get; }

        public ServiceJourneyRulesConfiguration(ILogger<ServiceConfigurationModule> logger,
            IConfiguration configuration)
        {
            if (Program.Mode == Program.RunMode.Validate)
            {
                var configurationFolderPath = configuration.GetOrThrow("CONFIGURATION_FOLDER_PATH", logger);
                var journeysFolderName = configuration.GetOrThrow("JOURNEYS_FOLDER_NAME", logger);
                var rulesFolderName = configuration.GetOrThrow("RULES_FOLDER_NAME", logger);

                GpInfoFilePath = configuration.GetOrThrow("GP_INFO_FILE_PATH", logger);
                OutputFolderPath = configuration.GetOrThrow("OUTPUT_FOLDER_PATH", logger);
                RulesFolderPath = Path.Join(configurationFolderPath, rulesFolderName);
                JourneysFolderPath = Path.Join(configurationFolderPath, journeysFolderName);
            }
            else
            {
                InputFolderPath = configuration.GetOrThrow("INPUT_FOLDER_PATH", logger);
            }
        }
    }
}