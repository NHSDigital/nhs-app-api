namespace NHSOnline.Backend.Metrics.EventHub
{
    internal sealed class EventHubClientConfiguration : IEventHubClientConfiguration
    {
        public EventHubs EventHub { get; }

        public string ConnectionString { get; }

        public EventHubClientConfiguration(EventHubs eventHub, string connectionString)
        {
            EventHub = eventHub;
            ConnectionString = connectionString;
        }
    }
}