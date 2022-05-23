using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Compute.FirstLogins;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.ReferrerLogin
{
    internal static class ReferrerLoginServiceCollectionExtensions
    {
        internal static void AddReferrerLogin(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<IReferrerLoginCompute, ReferrerLoginCompute>();
        }
    }
}
