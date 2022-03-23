using Microsoft.Extensions.DependencyInjection;
using Moq;
using NHSOnline.MetricLogFunctionApp.Slack;
using NHSOnline.MetricLogFunctionApp.Monitoring;

namespace NHSOnline.MetricLogFunctionApp.UnitTests.Monitoring
{
    internal sealed class MonitoringServiceContext
    {
        private readonly AlertsMessageBuilderService _alertsMessageBuilderService = new AlertsMessageBuilderService();

        internal Mock<ISlackClient> SlackClient { get; } = new Mock<ISlackClient>();
        internal string SlackMonitoringChannel { get; } = "MonitoringChannel";

        internal MonitoringService MonitoringService;

        public MonitoringServiceContext()
        {
            var serviceCollection = new ServiceCollection();
            ConfigureServices(serviceCollection);
            MonitoringService = serviceCollection.BuildServiceProvider().GetRequiredService<MonitoringService>();
        }

        internal void ConfigureServices(IServiceCollection collection)
        {
            var config = new MonitoringConfig
            {
                SlackChannel = SlackMonitoringChannel
            };

            collection.AddMonitoring();
            collection
                .AddSingleton(config)
                .AddSingleton(SlackClient.Object)
                .AddSingleton(_alertsMessageBuilderService);
        }
    }
}