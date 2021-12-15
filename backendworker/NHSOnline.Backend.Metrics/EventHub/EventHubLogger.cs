using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Metrics.EventHub
{
    internal class EventHubLogger : IEventHubLogger
    {
        private readonly List<IEventHubClient> _eventHubClients;
        private readonly string _environmentName;

        public EventHubLogger(
            IEventHubLoggerConfiguration configuration,
            IEnumerable<IEventHubClient> eventHubClients)
        {
            _environmentName = configuration.EnvironmentName;
            _eventHubClients = eventHubClients.ToList();
        }

        public Task MessageCreated(MessageCreatedEventLogData data) => WriteEventLog(data, EventHubs.CommsHubBoth);
        public Task MessageRead(MessageReadEventLogData data) => WriteEventLog(data, EventHubs.CommsHubBoth);
        public Task NotificationEnqueued(NotificationEnqueuedEventLogData data) =>
            WriteEventLog(data, EventHubs.CommsHubBoth);

        private Task WriteEventLog(
            IEventLogData data,
            EventHubs eventHub,
            [CallerMemberName] string action = "")
        {
            var tasks = new List<Task>();

            foreach (var eventHubClient in _eventHubClients)
            {
                if (eventHub.HasFlag(eventHubClient.EventHub))
                {
                    var logData = EventHubLogBuilder
                        .Create(action, _environmentName)
                        .With(data, eventHubClient.PidAllowed)
                        .Build();

                    tasks.Add(eventHubClient.WriteToEventHub(logData));
                }
            }

            return Task.WhenAll(tasks);
        }
    }
}