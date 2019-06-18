using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Exceptions;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils.Steps;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    internal class JourneyService : IJourneyService
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _services;

        public JourneyService(IServiceProvider services, ILogger<JourneyService> logger)
        {
            _services = services;
            _logger = logger;
        }

        public async Task<IDictionary<string, Journeys>> GetJourneys()
        {
            var context = new LoadContext();
            var loadSteps = _services.GetServices<ILoadStep>()
                .OrderBy(s => s.Order)
                .ToList();

            for (var i = 0; i < loadSteps.Count; i++)
            {
                var loadStep = loadSteps[i];
                _logger.LogInformation($"Steps {i+1}/{loadSteps.Count}: {loadStep.Description}");
                
                if (!await loadStep.Execute(context))
                {
                    throw new FailedLoadJourneysException();
                }
            }
            
            return context.MergedOdsJourneys;
        }
    }
}