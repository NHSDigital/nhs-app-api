using System;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ServiceJourneyRulesApi.Exceptions;
using NHSOnline.Backend.ServiceJourneyRulesApi.Extensions;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.StartupFilters
{
    internal class JourneyRepositoryStartupFilter : IStartupFilter
    {
        private readonly IJourneyService _journeyService;
        private readonly IServiceCollection _service;
        private readonly IApplicationLifetime _applicationLifetime;

        public JourneyRepositoryStartupFilter(
            IJourneyService journeyService,
            IServiceCollection service,
            ILoggerFactory loggerFactory,
            IApplicationLifetime applicationLifetime)
        {
            // Add logging capability to journey service
            loggerFactory.AddConsole();

            _journeyService = journeyService;
            _service = service;
            _applicationLifetime = applicationLifetime;
        }

        public Action<IApplicationBuilder> Configure(Action<IApplicationBuilder> next)
        {
            return builder =>
            {
                try
                {
                    var response = _journeyService.GetJourneys();
                    response.Wait();

                    var journeyRepository = new JourneyRepository(response.Result);

                    _service.AddSingleton(journeyRepository);

                    next(builder);
                }
                catch (AggregateException exception)
                {
                    if (exception.InnerExceptions.Count != 1 ||
                        !(exception.InnerException is FailedLoadJourneysException))
                    {
                        throw;
                    }

                    _applicationLifetime.StopApplication(1);
                }
            };
        }
    }
}