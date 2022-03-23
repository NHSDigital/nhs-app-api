using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Load;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl
{
    internal static class EtlServiceCollectionExtensions
    {
        internal static void AddEtl(this IServiceCollection serviceCollection)
        {
            AddInfrastructure(serviceCollection);
            AddFunctions(serviceCollection);
        }

        private static void AddInfrastructure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddLoad();
            serviceCollection.AddEtlLogging();
        }

        private static void AddFunctions(IServiceCollection serviceCollection)
        {
            serviceCollection.AddAuditLog();
        }
    }
}