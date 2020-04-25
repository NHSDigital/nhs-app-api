using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    internal class ValidationService : BackgroundService
    {
        private readonly IConfigurationRuleFileValidator _configurationRuleFileValidator;
        private readonly IHostApplicationLifetime _applicationLifetime;
        
        public ValidationService(
            IConfigurationRuleFileValidator configurationRuleFileValidator,
            IHostApplicationLifetime applicationLifetime)
        {
            _configurationRuleFileValidator = configurationRuleFileValidator;
            _applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                Environment.ExitCode = await _configurationRuleFileValidator.ValidateJourneyConfigurationFiles();
            }
            
            _applicationLifetime.StopApplication();
        }
    }
}