namespace NHSOnline.Backend.Auditing
{
    public interface IEventHubAuditSinkConfiguration
    {
        string ConnectionString { get; }

        string EventHubName { get; }
    }
}