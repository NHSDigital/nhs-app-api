using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using Microsoft.Azure.Storage.Queue;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.Resilience
{
    public interface IRequestQueueOrchestrator<in TRequest>
    {
        public Task<HttpResponseMessage> QueueManualRequest(
            ILogger logger,
            HttpRequestMessage req,
            CloudQueue queue);

        public Task QueueScheduledRequest(
            ILogger logger,
            CloudQueue queue,
            Func<TRequest> reportRequestGenerator);

        public Task AddMessage(
            ILogger logger,
            CloudQueue processQueue,
            TRequest message);

        public Task AddMessage(
            ILogger logger,
            string queueName,
            TRequest message);
    }
}