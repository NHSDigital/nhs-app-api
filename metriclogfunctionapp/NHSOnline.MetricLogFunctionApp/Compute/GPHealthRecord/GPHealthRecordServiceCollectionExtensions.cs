using Microsoft.Extensions.DependencyInjection;

namespace NHSOnline.MetricLogFunctionApp.Compute.GPHealthRecord;

internal static class GPHealthRecordServiceCollectionExtensions
{
    internal static void AddGPHealthRecord(this IServiceCollection serviceCollection)
    {
        serviceCollection.AddTransient<IGPHealthRecordCompute, GPHealthRecordCompute>();
    }

}
