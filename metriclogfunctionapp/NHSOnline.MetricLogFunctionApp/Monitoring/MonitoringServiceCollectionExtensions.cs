using Microsoft.Extensions.DependencyInjection;
using NHSOnline.MetricLogFunctionApp.Monitoring.AlertFormatters;

namespace NHSOnline.MetricLogFunctionApp.Monitoring
{
    public static class MonitoringServiceCollectionExtensions
    {
        public static void AddMonitoring(this IServiceCollection serviceCollection)
        {
            serviceCollection.AddTransient<MonitoringService>();
            serviceCollection.AddTransient<SearchResultsMapper>();
            serviceCollection.AddTransient<IAlertFormatter, AuditLogUnprocessedEventsAlertFormatter>();
            serviceCollection.AddTransient<AlertsMessageBuilderService>();

            serviceCollection.AddConfig<MonitoringConfig>("Monitoring");
        }
    }
}