using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.Support;
using static NHSOnline.Backend.Support.ValidateAndLog.ValidationOptions;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps
{
    internal class ValidateUniqueOdsConfiguration : IValidatorStep
    {
        public string Description { get; } = "Validating unique ODS code per rule folder";
        public ProcessOrder Order { get; } = ProcessOrder.ValidateUniqueOdsConfiguration;

        private readonly ILogger _logger;

        public ValidateUniqueOdsConfiguration(ILogger<ValidateUniqueOdsConfiguration> logger)
        {
            _logger = logger;
        }

        public Task<bool> Execute(ConfigurationContext context)
        {
            new ValidateAndLog(_logger)
                .IsNotNull(context, nameof(context), ThrowError)
                .IsNotNull(context?.FolderConfigurations, nameof(context.FolderConfigurations), ThrowError)
                .IsNotNull(context?.GpInfos, nameof(context.GpInfos), ThrowError)
                .IsValid();

            if (!GetFolderOdsConfigurations(context.FolderConfigurations, context.GpInfos,
                out var folderOdsJourneys))
            {
                _logger.LogCritical(
                    "Error validating unique ODS code per folder. See output above for specific errors.");
                return Task.FromResult(false);
            }

            context.FolderOdsJourneys = folderOdsJourneys;
            return Task.FromResult(true);
        }

        private bool GetFolderOdsConfigurations(
            IDictionary<string, IEnumerable<TargetConfiguration>> folderConfigurations,
            IDictionary<string, GpInfo> gpInfos,
            out Dictionary<string, IDictionary<string, Journeys>> folderOdsJourneys)
        {
            var hasError = false;
            folderOdsJourneys = new Dictionary<string, IDictionary<string, Journeys>>();

            foreach (var (folderName, configurations) in folderConfigurations)
            {
                var (didError, uniqueOdsJourneys) = GetUniqueOdsJourneys(gpInfos, folderName, configurations);
                hasError = hasError || didError;

                folderOdsJourneys.Add(folderName, uniqueOdsJourneys);
            }

            return !hasError;
        }

        private (bool hadError, Dictionary<string, Journeys> odsConfigurations) GetUniqueOdsJourneys(
            IDictionary<string, GpInfo> gpInfos,
            string folderName,
            IEnumerable<TargetConfiguration> configurations)
        {
            var hasError = false;
            var uniqueOdsJourneys = new Dictionary<string, Journeys>();

            foreach (var configuration in configurations)
            {
                var targetConfigurations = GetTargetJourneys(gpInfos, configuration);

                hasError |= AddToOdsJourneys(targetConfigurations, uniqueOdsJourneys, folderName);
            }

            return (hasError, uniqueOdsJourneys);
        }

        private bool AddToOdsJourneys(
            IDictionary<string, Journeys> targetConfigurations,
            IDictionary<string, Journeys> uniqueOdsJourneys,
            string folderName)
        {
            var hasError = false;
            foreach (var (target, odsConfiguration) in targetConfigurations)
            {
                try
                {
                    uniqueOdsJourneys.Add(target, odsConfiguration);
                }
                catch (ArgumentException e)
                {
                    hasError = true;
                    _logger.LogError(e,
                        $"Error applying '{target}' ODS configuration to '{folderName}' list");
                }
            }

            return hasError;
        }

        private static IDictionary<string, Journeys> GetTargetJourneys(
            IDictionary<string, GpInfo> gpInfos,
            TargetConfiguration configuration)
        {
            var targets = GetTargets(gpInfos, configuration.Target);

            return targets.ToDictionary(t => t.Ods, t => configuration.Journeys.Clone().AddSupplier(t.Supplier.ToSupplier()));
        }

        private static IEnumerable<GpInfo> GetTargets(IDictionary<string, GpInfo> gpInfos, Target target)
        {
            if (!string.IsNullOrWhiteSpace(target.OdsCode))
            {
                return gpInfos.Where(i => string.Compare(i.Key, target.OdsCode, StringComparison.OrdinalIgnoreCase) == 0)
                    .Select(i=>i.Value);
            }

            if (target.OdsCodes != null)
            {
                return gpInfos.Where(i=>target.OdsCodes.Contains(i.Value.Ods)).Select(i=>i.Value);
            }

            var gpInfoTargets = gpInfos.AsEnumerable();

            switch (target)
            {
                case var _ when !string.IsNullOrWhiteSpace(target.CcgCode):
                    gpInfoTargets = gpInfoTargets.Where(i =>
                        string.Equals(i.Value.CcgCode, target.CcgCode, StringComparison.Ordinal));
                    break;
                case var _ when target.Supplier != default:
                    gpInfoTargets = gpInfoTargets.Where(i => i.Value.Supplier.ToSupplier() == target.Supplier);
                    break;
            }

            return gpInfoTargets.Select(i=>i.Value);
        }
    }
}