using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Compute.DailyDeviceReferralUsage;
using NHSOnline.MetricLogFunctionApp.Compute.FirstLogins;
using NHSOnline.MetricLogFunctionApp.Compute.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.ReferrerLogin;

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
        }

        private static void AddInfrastructure(IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient(typeof(ComputeExecutor<>));
            serviceCollection.AddTransient(typeof(IComputeExecutor<>), typeof(ComputeExecutor<>));
        }
    }
}