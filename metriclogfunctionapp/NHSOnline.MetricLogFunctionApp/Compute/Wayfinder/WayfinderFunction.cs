using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.Wayfinder
{
    public sealed class WayfinderFunction
    {
        private const string QueueName = "wayfinder-%QueueNameSuffix%";

        private readonly ILogger _logger;
        private readonly IWayfinderCompute _compute;
        private readonly IComputeExecutor<WayfinderComputeRequest> _executor;
        private readonly IRequestQueueOrchestrator<WayfinderComputeRequest> _queueOrchestrator;

        public WayfinderFunction(
            ILogger<WayfinderFunction> logger,
            IWayfinderCompute compute,
            IComputeExecutor<WayfinderComputeRequest> executor,
            IRequestQueueOrchestrator<WayfinderComputeRequest> queueOrchestrator)
        {
            _compute = compute;
            _executor = executor;
            _queueOrchestrator = queueOrchestrator;
            _logger = logger;
        }

        [FunctionName("Wayfinder_Compute_Http")]
        public async Task<HttpResponseMessage> Http(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req,
            [Queue(QueueName)] CloudQueue queue)
        {
            return await _queueOrchestrator.QueueManualRequest(_logger, req, queue);
        }

        [FunctionName("Wayfinder_Compute_Queue")]
        public async Task Queue(
            [QueueTrigger(QueueName)] CloudQueueMessage message,
            [Queue(QueueName)] CloudQueue queue,
            [Queue(QueueName + "-poison")] CloudQueue poisonQueue)
        {
            await _executor.Execute(_logger, message, queue, poisonQueue, ExecuteCompute);
        }

        [FunctionName("Wayfinder_Compute_Timer")]
        public async Task Timer(
            [TimerTrigger(RunDaily.CronExpressionEarlyMorning)]
            TimerInfo timerInfo,
            [Queue(QueueName)] CloudQueue queue)
        {
            await _queueOrchestrator.QueueScheduledRequest(_logger, queue, CreateDailyScheduledRequest);

            WayfinderComputeRequest CreateDailyScheduledRequest()
            {
                var (startDateTime, endDateTime) = RunDaily.CalculateRunDateTimeRange();

                var request = new WayfinderComputeRequest
                {
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime
                };

                return request;
            }
        }

        private async Task ExecuteCompute(WayfinderComputeRequest request)
        {
            await _compute.Execute(request.StartDateTime.Date, request.EndDateTime.Date);
        }
    }
}
