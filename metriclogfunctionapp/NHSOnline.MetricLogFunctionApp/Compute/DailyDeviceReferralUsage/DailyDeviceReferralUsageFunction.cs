using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.DailyDeviceReferralUsage
{
    public sealed class DailyDeviceReferralUsageFunction
    {
        private const string QueueName = "daily-device-referrals-metric-%QueueNameSuffix%";

        private readonly ILogger<DailyDeviceReferralUsageFunction> _logger;
        private readonly IDailyDeviceReferralUsageCompute _compute;
        private readonly IComputeExecutor<DailyDeviceReferralUsageReportRequest> _executor;
        private readonly IRequestQueueOrchestrator<DailyDeviceReferralUsageReportRequest> _queueOrchestrator;

        public DailyDeviceReferralUsageFunction(
            ILogger<DailyDeviceReferralUsageFunction> logger,
            IDailyDeviceReferralUsageCompute compute,
            IComputeExecutor<DailyDeviceReferralUsageReportRequest> executor,
            IRequestQueueOrchestrator<DailyDeviceReferralUsageReportRequest> queueOrchestrator)
        {
            _compute = compute;
            _executor = executor;
            _queueOrchestrator = queueOrchestrator;
            _logger = logger;
        }

        [FunctionName("DailyDeviceReferralUsage_Compute_Http")]
        public async Task<HttpResponseMessage> Http(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req,
            [Queue(QueueName)] CloudQueue queue)
        {
            return await _queueOrchestrator.QueueManualRequest(_logger, req, queue);
        }

        [FunctionName("DailyDeviceReferralUsage_Compute_Timer")]
        public async Task Timer(
            [TimerTrigger(RunDaily.CronExpressionEarlyMorningOffset)]
            TimerInfo timerInfo,
            [Queue(QueueName)] CloudQueue queue)
        {
            await _queueOrchestrator.QueueScheduledRequest(_logger, queue, CreateDailyScheduledRequest);

            DailyDeviceReferralUsageReportRequest CreateDailyScheduledRequest()
            {
                var (startDateTime, endDateTime) = RunDaily.CalculateRunDateTimeRange();

                var request = new DailyDeviceReferralUsageReportRequest
                {
                    StartDateTime = startDateTime,
                    EndDateTime = endDateTime
                };

                return request;
            }
        }

        [FunctionName("DailyDeviceReferralUsage_Compute_Queue")]
        public async Task Queue(
            [QueueTrigger(QueueName)] CloudQueueMessage message,
            [Queue(QueueName)] CloudQueue queue,
            [Queue(QueueName + "-poison")] CloudQueue poisonQueue)
        {
            await _executor.Execute(_logger, message, queue, poisonQueue, ExecuteCompute);
        }

        private async Task ExecuteCompute(DailyDeviceReferralUsageReportRequest request)
        {
            await _compute.Execute(request.StartDateTime, request.EndDateTime);
        }
    }
}