using System.IO;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    internal class ServiceJourneyRulesConfiguration : IServiceJourneyRulesConfiguration
    {
        public string GpInfoFilePath { get; }
        public string RulesFolderPath { get; }
        public string JourneysFolderPath { get; }

        public ServiceJourneyRulesConfiguration(ILogger<ServiceConfigurationModule> logger, IConfiguration configuration)
        {
            var configurationFolderPath = configuration.GetOrThrow("CONFIGURATION_FOLDER_PATH", logger);
            var rulesFolderName = configuration.GetOrThrow("RULES_FOLDER_NAME", logger);
            var journeysFolderName = configuration.GetOrThrow("JOURNEYS_FOLDER_NAME", logger);

            GpInfoFilePath = configuration.GetOrThrow("GP_INFO_FILE_PATH", logger);
            RulesFolderPath = Path.Join(configurationFolderPath, rulesFolderName);
            JourneysFolderPath = Path.Join(configurationFolderPath, journeysFolderName);
        }
    }
}