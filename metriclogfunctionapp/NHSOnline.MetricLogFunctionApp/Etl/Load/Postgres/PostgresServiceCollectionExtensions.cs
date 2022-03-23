using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Etl.Load.Postgres
{
    internal static class PostgresServiceCollectionExtensions
    {
        internal static void AddPostgres(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IEventsRepository, PostgresEventsRepository>();
        }
    }
}