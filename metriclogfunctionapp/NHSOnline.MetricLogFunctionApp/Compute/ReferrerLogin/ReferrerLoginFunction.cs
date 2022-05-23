using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.ReferrerLogin
{
    public sealed class ReferrerLoginFunction
    {
        private const string QueueName = "referrer-login-%QueueNameSuffix%";

        private readonly ILogger _logger;
        private readonly IReferrerLoginCompute _compute;
        private readonly IComputeExecutor<ReferrerLoginReportRequest> _executor;
        private readonly IRequestQueueOrchestrator<ReferrerLoginReportRequest> _queueOrchestrator;

        public ReferrerLoginFunction(
            ILogger<ReferrerLoginFunction> logger,
            IReferrerLoginCompute compute,
            IComputeExecutor<ReferrerLoginReportRequest> executor,
            IRequestQueueOrchestrator<ReferrerLoginReportRequest> queueOrchestrator)
        {
            _compute = compute;
            _executor = executor;
            _queueOrchestrator = queueOrchestrator;
            _logger = logger;
        }

        [FunctionName("ReferrerLogin_Compute_Http")]
        public async Task<HttpResponseMessage> Http(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req,
            [Queue(QueueName)] CloudQueue queue)
        {
            return await _queueOrchestrator.QueueManualRequest(_logger, req, queue);
        }

        [FunctionName("ReferrerLogin_Compute_Queue")]
        public async Task Queue(
            [QueueTrigger(QueueName)] CloudQueueMessage message,
            [Queue(QueueName)] CloudQueue queue,
            [Queue(QueueName + "-poison")] CloudQueue poisonQueue)
        {
            await _executor.Execute(_logger, message, queue, poisonQueue, ExecuteCompute);
        }

        [FunctionName("ReferrerLogin_Compute_Timer")]
        public async Task Timer(
            [TimerTrigger(RunDaily.CronExpressionEarlyMorning)]
            TimerInfo timerInfo,
            [Queue(QueueName)] CloudQueue queue)
        {
            await _queueOrchestrator.QueueScheduledRequest(_logger, queue, CreateDailyScheduledRequest);

            ReferrerLoginReportRequest CreateDailyScheduledRequest()
            {
                var (startDateTime, endDateTime) = RunDaily.CalculateRunDateTimeRange();

                var request = new ReferrerLoginReportRequest
                {
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime
                };

                return request;
            }
        }

        private async Task ExecuteCompute(ReferrerLoginReportRequest request)
        {
            await _compute.Execute(request.StartDateTime.Date, request.EndDateTime.Date);
        }
    }
}
