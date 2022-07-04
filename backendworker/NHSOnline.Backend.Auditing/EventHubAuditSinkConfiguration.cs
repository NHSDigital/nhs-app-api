namespace NHSOnline.Backend.Auditing
{
    public class EventHubAuditSinkConfiguration : IEventHubAuditSinkConfiguration
    {
        public string ConnectionString { get; }

        public string EventHubName { get; }

        internal EventHubAuditSinkConfiguration(string connectionString, string eventHubName)
        {
            ConnectionString = connectionString;
            EventHubName = eventHubName;
        }
    }
}