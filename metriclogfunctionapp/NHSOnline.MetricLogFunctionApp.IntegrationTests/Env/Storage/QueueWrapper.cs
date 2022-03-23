using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.Azure.Storage.Queue;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace NHSOnline.MetricLogFunctionApp.IntegrationTests.Env.Storage
{
    internal sealed class QueueWrapper
    {
        private static readonly TimeSpan WaitUntilEmptyTimeout = TimeSpan.FromSeconds(15);

        public QueueWrapper(TestLogs logs, CloudQueue queue)
        {
            Logs = logs;
            Queue = queue;
        }

        internal QueueWrapper Poison => new QueueWrapper(Logs, Client.GetQueueReference(PoisonQueueName));

        private TestLogs Logs { get; }
        private CloudQueue Queue { get; }

        private CloudQueueClient Client => Queue.ServiceClient;
        internal string Name => Queue.Name;
        private string PoisonQueueName => $"{Name}-poison";

        internal async Task WaitUntilEmpty(TimeSpan? timeout = null)
        {
            timeout ??= WaitUntilEmptyTimeout;

            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    await Queue.FetchAttributesAsync();
                    Queue.ApproximateMessageCount.Should().Be(0, $"{Name} messages should have been processed");

                    stopwatch.Stop();
                    Logs.Info("Queue {0} cleared after {1}", Queue.Name, stopwatch.Elapsed);

                    return;
                }
                catch (AssertFailedException) when (stopwatch.Elapsed < timeout)
                {
                    await Task.Delay(TimeSpan.FromMilliseconds(100));
                }
            }
        }

        internal async Task<List<CloudQueueMessage>> FetchAll()
        {
            var allMessages = new List<CloudQueueMessage>();

            List<CloudQueueMessage> messages;
            do
            {
                messages = (await Queue.GetMessagesAsync(32)).ToList();
                foreach (var message in messages)
                {
                    allMessages.Add(message);
                    await Queue.DeleteMessageAsync(message);
                }
            } while (messages.Count > 0);

            return allMessages;
        }

        public async Task AddJson(object data)
        {
            await EnsureExists();

            var json = JsonConvert.SerializeObject(data);
            await Queue.AddMessageAsync(new CloudQueueMessage(json));
        }

        public async Task EnsureExists() => await Queue.CreateIfNotExistsAsync();
    }
}