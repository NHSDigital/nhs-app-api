using System;
using Microsoft.Azure.Storage;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Extensions.Logging;

namespace NHSOnline.MetricLogFunctionApp.Resilience
{
    internal static class QueueFactory
    {
        private static readonly string AzureStorageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private static readonly string QueueNameSuffix = Environment.GetEnvironmentVariable("QueueNameSuffix");

        internal static CloudQueue FindQueue(ILogger logger, string queueName)
        {
            try
            {
                logger.LogInformation("Find queue");
                CloudQueueClient queueClient = CloudStorageAccount.Parse(AzureStorageConnectionString).CreateCloudQueueClient();
                var queue = queueClient.GetQueueReference(queueName + QueueNameSuffix);
                queue.CreateIfNotExists();

                if (queue.Exists())
                {
                    logger.LogInformation("Queue found: {QueueName}",queueName);
                    return queue;
                }
                logger.LogError("Make sure the queue {QueueName} exists",queueName);
                return null;
            }
            catch (Exception ex)
            {
                logger.LogError("Exception: {Message}\n\n", ex.Message);
                return null;
            }
        }
    }
}