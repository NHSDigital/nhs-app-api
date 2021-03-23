namespace NHSOnline.Backend.Metrics.EventHub
{
    public interface IEventHubClientConfiguration
    {
        EventHubs EventHub { get; }

        string ConnectionString { get; }
    }
}