using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.ReferrerServiceJourney
{
    public sealed class ReferrerServiceJourneyFunction
    {
        private const string QueueName = "referrer-servicejourney-%QueueNameSuffix%";

        private readonly ILogger _logger;
        private readonly IReferrerServiceJourneyCompute _compute;
        private readonly IComputeExecutor<ReferrerServiceJourneyReportRequest> _executor;
        private readonly IRequestQueueOrchestrator<ReferrerServiceJourneyReportRequest> _queueOrchestrator;

        public ReferrerServiceJourneyFunction(
            ILogger<ReferrerServiceJourneyFunction> logger,
            IReferrerServiceJourneyCompute compute,
            IComputeExecutor<ReferrerServiceJourneyReportRequest> executor,
            IRequestQueueOrchestrator<ReferrerServiceJourneyReportRequest> queueOrchestrator)
        {
            _compute = compute;
            _executor = executor;
            _queueOrchestrator = queueOrchestrator;
            _logger = logger;
        }

        [FunctionName("ReferrerServiceJourney_Compute_Http")]
        public async Task<HttpResponseMessage> Http(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req,
            [Queue(QueueName)] CloudQueue queue)
        {
            return await _queueOrchestrator.QueueManualRequest(_logger, req, queue);
        }

        [FunctionName("ReferrerServiceJourney_Compute_Queue")]
        public async Task Queue(
            [QueueTrigger(QueueName)] CloudQueueMessage message,
            [Queue(QueueName)] CloudQueue queue,
            [Queue(QueueName + "-poison")] CloudQueue poisonQueue)
        {
            await _executor.Execute(_logger, message, queue, poisonQueue, ExecuteCompute);
        }

        [FunctionName("ReferrerServiceJourney_Compute_Timer")]
        public async Task Timer(
            [TimerTrigger(RunDaily.CronExpressionEarlyMorning)]
            TimerInfo timerInfo,
            [Queue(QueueName)] CloudQueue queue)
        {
            await _queueOrchestrator.QueueScheduledRequest(_logger, queue, CreateDailyScheduledRequest);

            ReferrerServiceJourneyReportRequest CreateDailyScheduledRequest()
            {
                var (startDateTime, endDateTime) = RunDaily.CalculateRunDateTimeRange();

                var request = new ReferrerServiceJourneyReportRequest
                {
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime
                };

                return request;
            }
        }

        private async Task ExecuteCompute(ReferrerServiceJourneyReportRequest request)
        {
            await _compute.Execute(request.StartDateTime.Date, request.EndDateTime.Date);
        }
    }
}
