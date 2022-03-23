using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Etl.Load.Postgres;

namespace NHSOnline.MetricLogFunctionApp.Etl.Load
{
    internal static class LoadServiceCollectionExtensions
    {
        internal static void AddLoad(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddPostgres();
        }
    }
}