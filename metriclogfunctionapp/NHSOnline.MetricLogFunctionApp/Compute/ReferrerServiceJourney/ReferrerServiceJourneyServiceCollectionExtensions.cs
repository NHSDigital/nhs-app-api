using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Compute.ReferrerServiceJourney
{
    internal static class ReferrerServiceJourneyServiceCollectionExtensions
    {
        internal static void AddReferrerServiceJourney(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IReferrerServiceJourneyCompute, ReferrerServiceJourneyCompute>();
        }
    }
}
