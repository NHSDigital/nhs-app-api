using System;
using System.Text;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;

namespace NHSOnline.Backend.Metrics.EventHub
{
    internal sealed class EventHubClient : IEventHubClient, IAsyncDisposable
    {
        private readonly EventHubProducerClient _producerClient;

        public EventHubs EventHub { get; }
        public bool PidAllowed { get; }

        public EventHubClient(IEventHubClientConfiguration configuration)
        {
            _producerClient = new EventHubProducerClient(configuration.ConnectionString);
            EventHub = configuration.EventHub;
            PidAllowed = configuration.EventHub.HasFlag(EventHubs.CommsHubPid);
        }

        public async Task WriteToEventHub(string eventLog)
        {
            using var eventBatch = await _producerClient.CreateBatchAsync();

            eventBatch.TryAdd(new EventData(Encoding.UTF8.GetBytes(eventLog)));

            await _producerClient.SendAsync(eventBatch);
        }

        public async ValueTask DisposeAsync()
        {
            if (_producerClient != null)
            {
                await _producerClient.DisposeAsync().ConfigureAwait(false);
            }
        }
    }
}