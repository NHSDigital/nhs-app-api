using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.DeviceInfo;

public sealed class DeviceInfoFunction
{
    private const string QueueName = "device-info-%QueueNameSuffix%";

    private readonly ILogger _logger;
    private readonly IDeviceInfoCompute _compute;
    private readonly IComputeExecutor<DeviceInfoReportRequest> _executor;
    private readonly IRequestQueueOrchestrator<DeviceInfoReportRequest> _queueOrchestrator;

    public DeviceInfoFunction(
        ILogger<DeviceInfoFunction> logger,
        IDeviceInfoCompute compute,
        IComputeExecutor<DeviceInfoReportRequest> executor,
        IRequestQueueOrchestrator<DeviceInfoReportRequest> queueOrchestrator)
    {
        _compute = compute;
        _executor = executor;
        _queueOrchestrator = queueOrchestrator;
        _logger = logger;
    }

    [FunctionName("DeviceInfo_Compute_Http")]
    public async Task<HttpResponseMessage> Http(
        [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req,
        [Queue(QueueName)] CloudQueue queue)
    {
        return await _queueOrchestrator.QueueManualRequest(_logger, req, queue);
    }

    [FunctionName("DeviceInfo_Compute_Queue")]
    public async Task Queue(
        [QueueTrigger(QueueName)] CloudQueueMessage message,
        [Queue(QueueName)] CloudQueue queue,
        [Queue(QueueName + "-poison")] CloudQueue poisonQueue)
    {
        await _executor.Execute(_logger, message, queue, poisonQueue, ExecuteCompute);
    }

    [FunctionName("DeviceInfo_Compute_Timer")]
    public async Task Timer(
        [TimerTrigger(RunDaily.CronExpressionEarlyMorning)]
            TimerInfo timerInfo,
        [Queue(QueueName)] CloudQueue queue)
    {
        await _queueOrchestrator.QueueScheduledRequest(_logger, queue, CreateDailyScheduledRequest);

        DeviceInfoReportRequest CreateDailyScheduledRequest()
        {
            var (startDateTime, endDateTime) = RunDaily.CalculateRunDateTimeRange();

            var request = new DeviceInfoReportRequest
            {
                StartDateTime = startDateTime,
                EndDateTime = endDateTime
            };

            return request;
        }
    }

    private async Task ExecuteCompute(DeviceInfoReportRequest request)
    {
        await _compute.Execute(request.StartDateTime.Date, request.EndDateTime.Date);
    }
}
