using System;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.Auditing
{
    public class EventHubAuditorSink : IAuditSink
    {
        private readonly ILogger<EventHubAuditorSink> _logger;
        private readonly EventHubProducerClient _eventHubProducerClient;

        public EventHubAuditorSink(ILogger<EventHubAuditorSink> logger, EventHubProducerClient eventHubProducerClient)
        {
            _logger = logger;
            _eventHubProducerClient = eventHubProducerClient;
        }

        public async Task WritePreOperationAudit(AuditRecord auditRecord)
        {
            _logger.LogEnter();

            try
            {
                var (success, writeException) = await WriteAudit(auditRecord);

                if (success)
                {
                    return;
                }

                throw new PreOperationAuditException(
                    $"Error writing PreAudit Operation: {auditRecord.Operation}",
                    writeException);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        public async Task WritePostOperationAudit(AuditRecord auditRecord)
        {
            _logger.LogEnter();
            await WriteAudit(auditRecord);
            _logger.LogExit();
        }

        private async Task<(bool, Exception)> WriteAudit(AuditRecord auditRecord)
        {
            try
            {
                using var eventBatch = await _eventHubProducerClient.CreateBatchAsync(
                    new CreateBatchOptions
                    {
                        PartitionKey = auditRecord.AuditId
                    }
                );
                
                auditRecord.Timestamp = DateTime.UtcNow;

                var eventData = GetEventData(auditRecord);
                
                if (!eventBatch.TryAdd(eventData))
                {
                    return (
                        false,
                        new EventHubAuditException("Failed to add audit record to event hub batch")
                    );
                }

                await _eventHubProducerClient.SendAsync(eventBatch);

                return (true, null);
            }
            catch (Exception e)
            {
                return (false, e);
            }
        }

        private static EventData GetEventData(AuditRecord auditRecord)
        {
            return new EventData(
                new BinaryData(
                    JsonSerializer.Serialize(auditRecord)
                )
            );
        }
    }
}