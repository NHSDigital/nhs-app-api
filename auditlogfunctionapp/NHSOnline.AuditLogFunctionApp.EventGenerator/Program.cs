using NHSOnline.AuditLogFunctionApp.Model;

using System;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

using Azure.Messaging.EventHubs;
using Azure.Messaging.EventHubs.Producer;
using Microsoft.Extensions.Configuration;

namespace NHSOnline.AuditLogFunctionApp.EventGenerator
{
    public static class Program
    {
        private static readonly Random Random = new Random();
        private static readonly JsonSerializerOptions SerializerOptions =
            new JsonSerializerOptions { WriteIndented = true };

        public static async Task Main()
        {
            var config = GetConfig();
            var eventHubProducerClient = BuildEventHubClient(config);
            var numberOfEvents = int.Parse(config["NumberOfEvents"]);

            try
            {
                await Task.WhenAll(
                    Enumerable.Range(1, numberOfEvents)
                        .AsParallel()
                        .Select(i => SendAuditEvent(eventHubProducerClient, i))
                        .ToArray()
                );
            }
            finally
            {
                await eventHubProducerClient.DisposeAsync();
            }
        }

        private static IConfigurationRoot GetConfig() =>
            new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .Build();

        private static EventHubProducerClient BuildEventHubClient(IConfigurationRoot config) =>
            new EventHubProducerClient(config["ConnectionString"], config["EventHubName"]);

        private static async Task SendAuditEvent(EventHubProducerClient eventHubProducerClient, long index)
        {
            var auditRecord = GenerateAuditRecord();
            auditRecord.Id = auditRecord.AuditId;

            using var eventBatch = await eventHubProducerClient.CreateBatchAsync(
                new CreateBatchOptions
                {
                    PartitionKey = auditRecord.AuditId
                }
            );

            var eventData = GetEventData(auditRecord);

            if (!eventBatch.TryAdd(eventData))
            {
                // if it is too large for the batch
                await Console.Error.WriteLineAsync($"Failed to send audit event #{index}");
                return;
            }

            await eventHubProducerClient.SendAsync(eventBatch);

            await Console.Out.WriteLineAsync(JsonSerializer.Serialize(auditRecord, SerializerOptions));
        }

        private static EventData GetEventData(AuditRecord auditRecord) =>
            new EventData(
                new BinaryData(
                    JsonSerializer.Serialize(auditRecord)
                )
            );

        private static AuditRecord GenerateAuditRecord() =>
            new AuditRecord(
                DateTime.UtcNow,
                Guid.NewGuid().ToString(),
                GenerateNHSNumber(),
                false,
                "Emis",
                "OnlineConsultations_Submitted",
                "Online consultations submitted",
                new VersionTag(null, null, null),
                Guid.NewGuid().ToString(),
                "test",
                "testIntegrationReferrer",
                "sessionId",
                "P9"
            );

        private static string GenerateNHSNumber() =>
            Random.Next(1000000000, 2000000000).ToString();
    }
}
