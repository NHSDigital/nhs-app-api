namespace NHSOnline.Backend.Metrics.EventHub
{
    internal sealed class EventHubLoggerConfiguration : IEventHubLoggerConfiguration
    {
        public EventHubLoggerConfiguration(string environmentName)
        {
            EnvironmentName = environmentName;
        }

        public string EnvironmentName { get; }
    }
}