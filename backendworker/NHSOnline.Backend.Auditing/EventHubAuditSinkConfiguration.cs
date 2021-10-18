using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Repository;
using NHSOnline.Backend.Support.Settings;

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