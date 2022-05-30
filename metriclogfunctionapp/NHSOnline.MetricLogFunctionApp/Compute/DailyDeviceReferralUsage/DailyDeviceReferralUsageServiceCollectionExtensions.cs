using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.DailyDeviceReferralUsage
{
    internal static class DailyDeviceReferralUsageServiceCollectionExtensions
    {
        internal static void AddDailyDeviceReferralUsage(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IDailyDeviceReferralUsageCompute, DailyDeviceReferralUsageCompute>();
            serviceCollection.AddTransient<IComputeExecutor<DailyDeviceReferralUsageReportRequest>, ComputeExecutor<DailyDeviceReferralUsageReportRequest>>();
            serviceCollection.AddTransient<IRequestQueueOrchestrator<DailyDeviceReferralUsageReportRequest>, RequestQueueOrchestrator<DailyDeviceReferralUsageReportRequest>>();
        }
    }
}