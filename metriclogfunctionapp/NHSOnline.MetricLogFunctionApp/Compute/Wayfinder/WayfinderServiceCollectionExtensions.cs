using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Compute.Wayfinder
{
    internal static class WayfinderServiceCollectionExtensions
    {
        internal static void AddWayfinder(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IWayfinderCompute, WayfinderCompute>();
        }
    }
}
