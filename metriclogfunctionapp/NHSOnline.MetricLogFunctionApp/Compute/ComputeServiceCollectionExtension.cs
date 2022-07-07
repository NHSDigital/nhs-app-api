using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Compute.DailyDeviceReferralUsage;
using NHSOnline.MetricLogFunctionApp.Compute.FirstLogins;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.ReferrerLogin;
using NHSOnline.MetricLogFunctionApp.Compute.ReferrerServiceJourney;
using NHSOnline.MetricLogFunctionApp.Compute.Wayfinder;

namespace NHSOnline.MetricLogFunctionApp.Compute
{
    internal static class ComputeServiceCollectionExtension
    {
        internal static void AddCompute(this IServiceCollection serviceCollection)
        {
            AddFunctions(serviceCollection);
            AddInfrastructure(serviceCollection);
            serviceCollection.AddComputeLogging();
        }

        private static void AddFunctions(IServiceCollection serviceCollection)
        {
            serviceCollection.AddFirstLogins();
            serviceCollection.AddReferrerLogin();
            serviceCollection.AddDailyDeviceReferralUsage();
            serviceCollection.AddReferrerServiceJourney();
            serviceCollection.AddWayfinder();
        }

        private static void AddInfrastructure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(ComputeExecutor<>));
            serviceCollection.AddTransient(typeof(IComputeExecutor<>), typeof(ComputeExecutor<>));
        }
    }
}
