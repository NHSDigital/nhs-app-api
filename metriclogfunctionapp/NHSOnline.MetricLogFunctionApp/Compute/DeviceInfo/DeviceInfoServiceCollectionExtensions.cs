using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Compute.DeviceInfo;

internal static class DeviceInfoServiceCollectionExtensions
{
    internal static void AddDeviceInfo(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IDeviceInfoCompute, DeviceInfoCompute>();
    }
}
