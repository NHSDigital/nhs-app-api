using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils
{
    internal class ConfigurationRuleFileValidator : IConfigurationRuleFileValidator
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;

        public ConfigurationRuleFileValidator(IServiceProvider services, ILogger<ConfigurationRuleFileValidator> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task<int> ValidateJourneyConfigurationFiles()
        {
            var context = new ConfigurationContext();
            var validatorSteps = _services.GetServices<IValidatorStep>()
                .OrderBy(s => s.Order)
                .ToList();

            for (var i = 0; i < validatorSteps.Count; i++)
            {
                var validatorStep = validatorSteps[i];
                _logger.LogInformation($"Steps {i+1}/{validatorSteps.Count}: {validatorStep.Description}");
                
                if (!await validatorStep.Execute(context))
                {
                    return 1;
                }
            }

            return 0;
        }
    }
}