using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.ServiceJourneyRulesApi.Exceptions;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;

namespace NHSOnline.Backend.ServiceJourneyRulesApi.Extensions
{
    internal static class ServiceCollectionExtensions
    {
        internal static void ConfigureJourneyRepository(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var journeyService = serviceProvider.GetService<IJourneyService>();

            try
            {
                var response = journeyService.GetJourneys();
                response.Wait();

                var journeyRepository = new JourneyRepository(response.Result);

                services.AddSingleton<IJourneyRepository>(journeyRepository);
            }
            catch (AggregateException exception)
            {
                if (exception.InnerExceptions.Count != 1 ||
                    !(exception.InnerException is FailedLoadJourneysException))
                {
                    throw;
                }

                Environment.Exit(1);
            }
        }
    }
}