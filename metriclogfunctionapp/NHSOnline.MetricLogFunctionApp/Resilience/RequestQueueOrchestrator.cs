using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using Microsoft.Azure.Storage.Queue;

namespace NHSOnline.MetricLogFunctionApp.Resilience
{
    public sealed class RequestQueueOrchestrator<TRequest> : IRequestQueueOrchestrator<TRequest>
    {
        private readonly string _azureStorageConnectionString = Environment.GetEnvironmentVariable("AzureWebJobsStorage");
        private readonly string _queueNameSuffix = Environment.GetEnvironmentVariable("QueueNameSuffix");

        public async Task<HttpResponseMessage> QueueManualRequest(
            ILogger logger,
            HttpRequestMessage req,
            CloudQueue queue)
        {
            logger.LogEnter();
            try
            {
                var body = await req.Content.ReadAsStringAsync();
                await queue.AddMessageAsync(new CloudQueueMessage(body));
                return new HttpResponseMessage(HttpStatusCode.Created);
            }
            catch (Exception e)
            {
                logger.LogMethodFailure(e);
                return new HttpResponseMessage(HttpStatusCode.InternalServerError);
            }
            finally
            {
                logger.LogExit();
            }
        }

        public async Task QueueScheduledRequest(
            ILogger logger,
            CloudQueue queue,
            Func<TRequest> reportRequestGenerator)
        {
            logger.LogEnter();
            try
            {
                var request = reportRequestGenerator();
                var messageContent = JsonConvert.SerializeObject(request);
                logger.LogInformation("Queuing request {Request}", messageContent);

                await queue.AddMessageAsync(new CloudQueueMessage(messageContent));
            }
            catch (Exception e)
            {
                logger.LogMethodFailure(e);
                throw;
            }
            finally
            {
                logger.LogExit();
            }
        }

        public async Task AddMessage(ILogger logger, CloudQueue processQueue, TRequest message)
        {
            var json = JsonConvert.SerializeObject(message);

            await processQueue.AddMessageAsync(new CloudQueueMessage(json));
            logger.LogInformation("Queued Message to {QueueName} with message {Message}", processQueue.Name, json);
        }

        public async Task AddMessage(ILogger logger, string queueName, TRequest message)
        {
            await AddMessage(logger, QueueFactory.FindQueue(logger, queueName), message);
        }
    }
}