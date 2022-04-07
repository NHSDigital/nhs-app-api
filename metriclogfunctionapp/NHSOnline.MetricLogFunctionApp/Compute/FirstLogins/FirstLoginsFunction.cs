using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.Storage.Queue;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using NHSOnline.MetricLogFunctionApp.Compute.QueueRequests;
using NHSOnline.MetricLogFunctionApp.Resilience;

namespace NHSOnline.MetricLogFunctionApp.Compute.FirstLogins
{
    public sealed class FirstLoginsFunction
    {
        private const string QueueName = "first-logins-metric-%QueueNameSuffix%";

        private readonly ILogger<FirstLoginsFunction> _logger;
        private readonly IFirstLoginsCompute _compute;
        private readonly IComputeExecutor<AuditReportRequest> _executor;
        private readonly IRequestQueueOrchestrator<AuditReportRequest> _queueOrchestrator;

        public FirstLoginsFunction(
            ILogger<FirstLoginsFunction> logger,
            IFirstLoginsCompute compute,
            IComputeExecutor<AuditReportRequest> executor,
            IRequestQueueOrchestrator<AuditReportRequest> queueOrchestrator)
        {
            _compute = compute;
            _executor = executor;
            _queueOrchestrator = queueOrchestrator;
            _logger = logger;
        }

        [FunctionName("FirstLogins_Compute_Http")]
        public async Task<HttpResponseMessage> Http(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req,
            [Queue(QueueName)] CloudQueue queue)
        {
            return await _queueOrchestrator.QueueManualRequest(_logger, req, queue);
        }

        [FunctionName("FirstLogins_Compute_Queue")]
        public async Task Queue(
            [QueueTrigger(QueueName)] CloudQueueMessage message,
            [Queue(QueueName)] CloudQueue queue,
            [Queue(QueueName + "-poison")] CloudQueue poisonQueue)
        {
            await _executor.Execute(_logger, message, queue, poisonQueue, ExecuteCompute);
        }

        private async Task ExecuteCompute(AuditReportRequest request)
        {
            await _compute.Execute(request.LoginId, request.StartDateTime, request.EndDateTime);
        }
    }
}