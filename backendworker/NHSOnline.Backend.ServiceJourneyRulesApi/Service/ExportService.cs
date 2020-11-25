using Microsoft.Extensions.Hosting;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Service
{
    internal class ExportService : BackgroundService
    {
        private readonly IHostApplicationLifetime _applicationLifetime;
        private readonly IConfigurationExporter _configurationExporter;

        public ExportService(
            IConfigurationExporter configurationExporter,
            IHostApplicationLifetime applicationLifetime)
        {
            _configurationExporter = configurationExporter;
            _applicationLifetime = applicationLifetime;
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!stoppingToken.IsCancellationRequested)
            {
                Environment.ExitCode = await _configurationExporter.Export();
            }

            _applicationLifetime.StopApplication();
        }
    }
}
