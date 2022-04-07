using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.FirstLogins
{
    internal static class FirstLoginsServiceCollectionExtensions
    {
        internal static void AddFirstLogins(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IFirstLoginsCompute, FirstLoginsCompute>();
            serviceCollection.AddTransient<IComputeExecutor<AuditReportRequest>, ComputeExecutor<AuditReportRequest>>();
            serviceCollection.AddTransient<IRequestQueueOrchestrator<AuditReportRequest>, RequestQueueOrchestrator<AuditReportRequest>>();
        }
    }
}