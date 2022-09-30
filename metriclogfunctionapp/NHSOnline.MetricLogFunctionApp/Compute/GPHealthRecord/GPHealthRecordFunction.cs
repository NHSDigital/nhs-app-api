using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.GPHealthRecord;

public sealed class GPHealthRecordFunction
{
    private const string QueueName = "gp-health-record-%QueueNameSuffix%";

    private readonly ILogger _logger;
    private readonly IGPHealthRecordCompute _compute;
    private readonly IComputeExecutor<GPHealthRecordReportRequest> _executor;
    private readonly IRequestQueueOrchestrator<GPHealthRecordReportRequest> _queueOrchestrator;

    public GPHealthRecordFunction(
        ILogger<GPHealthRecordFunction> logger,
        IGPHealthRecordCompute compute,
        IComputeExecutor<GPHealthRecordReportRequest> executor,
        IRequestQueueOrchestrator<GPHealthRecordReportRequest> queueOrchestrator)
    {
        _compute = compute;
        _executor = executor;
        _queueOrchestrator = queueOrchestrator;
        _logger = logger;
    }

    [FunctionName("GPHealthRecord_Compute_Http")]
    public async Task<HttpResponseMessage> Http(
        [HttpTrigger(AuthorizationLevel.Function, "post")]
        HttpRequestMessage req,
        [Queue(QueueName)] CloudQueue queue)
    {
        return await _queueOrchestrator.QueueManualRequest(_logger, req, queue);
    }

    [FunctionName("GPHealthRecord_Compute_Queue")]
    public async Task Queue(
        [QueueTrigger(QueueName)] CloudQueueMessage message,
        [Queue(QueueName)] CloudQueue queue,
        [Queue(QueueName + "-poison")] CloudQueue poisonQueue)
    {
        await _executor.Execute(_logger, message, queue, poisonQueue, ExecuteCompute);
    }

    [FunctionName("GPHealthRecord_Compute_Timer")]
    public async Task Timer(
        [TimerTrigger(RunDaily.CronExpressionEarlyMorning)]
        TimerInfo timerInfo,
        [Queue(QueueName)] CloudQueue queue)
    {
        await _queueOrchestrator.QueueScheduledRequest(_logger, queue, CreateDailyScheduledRequest);

        GPHealthRecordReportRequest CreateDailyScheduledRequest()
        {
            var (startDateTime, endDateTime) = RunDaily.CalculateRunDateTimeRange();

            var request = new GPHealthRecordReportRequest
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime
            };

            return request;
        }
    }

    private async Task ExecuteCompute(GPHealthRecordReportRequest request)
    {
        await _compute.Execute(request.StartDateTime.Date, request.EndDateTime.Date);
    }
}
