using System;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.ServiceJourneyRulesApi.Exceptions;
using NHSOnline.Backend.ServiceJourneyRulesApi.Extensions;
using NHSOnline.Backend.ServiceJourneyRulesApi.RuleConfiguration.Utils;
using NHSOnline.Backend.ServiceJourneyRulesApi.Service;
using IApplicationLifetime = Microsoft.Extensions.Hosting.IApplicationLifetime;

namespace NHSOnline.Backend.ServiceJourneyRulesApi
{
    internal static class ServiceCollectionExtensions
    {
        internal static void ConfigureJourneyRepository(this IServiceCollection services)
        {
            var serviceProvider = services.BuildServiceProvider();
            var journeyService = serviceProvider.GetService<IJourneyService>();
            var applicationLifetime = serviceProvider.GetService<IApplicationLifetime>();

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

                applicationLifetime.StopApplication(1);
            }
        }
    }
}