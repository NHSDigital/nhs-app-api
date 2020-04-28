using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal class MergeOdsJourneys : IValidatorStep
    {
        public string Description { get; } = "Merging ODS journeys";
        public ProcessOrder Order { get; } = ProcessOrder.MergeOdsJourneys;

        private readonly ILogger _logger;

        public MergeOdsJourneys(ILogger<MergeOdsJourneys> logger)
        {
            _logger = logger;
        }

        public Task<bool> Execute(ConfigurationContext context)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(context, nameof(context), ThrowError)
                .IsNotNull(context?.FolderOdsJourneys, nameof(context.FolderOdsJourneys), ThrowError)
                .IsValid();

            context.MergedOdsJourneys = Merge(context.FolderOdsJourneys);
            return Task.FromResult(true);
        }

        private Dictionary<string, Journeys> Merge(IDictionary<string, IDictionary<string, Journeys>> folderOdsJourneys)
        {
            var mergedOdsJourneys = new Dictionary<string, Journeys>();

            foreach (var (folderPath, odsJourneys) in folderOdsJourneys)
            {
                _logger.LogDebug($"Merging in {folderPath} journeys");
                MergeFolderJourneys(odsJourneys, mergedOdsJourneys);
            }

            return mergedOdsJourneys;
        }

        private static void MergeFolderJourneys(
            IDictionary<string, Journeys> odsJourneys,
            IDictionary<string, Journeys> mergedOdsJourneys)
        {
            foreach (var (odsCode, journeys) in odsJourneys)
            {
                if (!mergedOdsJourneys.TryGetValue(odsCode, out var currentJourneys))
                {
                    mergedOdsJourneys[odsCode] = journeys;
                    continue;
                }

                // Update existing ODS journeys
                currentJourneys.Merge(journeys);
            }
        }
    }
}
