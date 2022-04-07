using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Resilience
{
    internal static class ResilienceServiceCollectionExtensions
    {
        internal static void AddResilience(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(IRequestQueueOrchestrator<>),typeof(RequestQueueOrchestrator<>));
            serviceCollection.AddTransient(typeof(RequestQueueOrchestrator<>));
            serviceCollection.AddTransient(typeof(RequestParser<>));
        }
    }
}