using System;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Newtonsoft.Json;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Consent;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.Login;
using NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog.RegistrationAndLogin.WebIntegrationReferrals;
using NHSOnline.MetricLogFunctionApp.Etl.Logging;

namespace NHSOnline.MetricLogFunctionApp.Etl.Functions.AuditLog
{
    public class AuditLogConsumerFunction
    {
        private readonly IAuditLogEtl<ConsentMetric> _consentEtl;
        private readonly IAuditLogEtl<LoginMetric> _loginEtl;
        private readonly IAuditLogEtl<WebIntegrationReferralsMetric> _webIntegrationReferralEtl;
        private readonly IEtlLogger<AuditLogConsumerFunction> _logger;

        public AuditLogConsumerFunction(
            IAuditLogEtl<ConsentMetric> consentEtl,
            IAuditLogEtl<LoginMetric> loginEtl,
            IAuditLogEtl<WebIntegrationReferralsMetric> webIntegrationReferralEtl,
            IEtlLogger<AuditLogConsumerFunction> logger)
        {
            _consentEtl = consentEtl;
            _loginEtl = loginEtl;
            _webIntegrationReferralEtl = webIntegrationReferralEtl;
            _logger = logger;
        }

        [FunctionName("AuditLog_Etl_EventHub")]
        [ExponentialBackoffRetry(4, "00:00:15", "00:02:00")]
        public async Task AuditLog_Etl_EventHub(
            [EventHubTrigger(
                "%AuditEventHubName%",
                Connection = "AuditEventHubConnectionDataReceiver",
                ConsumerGroup = "%AuditEventHubAnalyticsConsumerGroup%")]
            AuditRecord[] events, ExecutionContext context)
        {
            await Execute(events, context);
        }

        [FunctionName("AuditLog_Etl_Http")]
        [ExponentialBackoffRetry(2, "00:00:01", "00:00:01")]
        public async Task<HttpResponseMessage> AuditLog_Etl_Http(
            [HttpTrigger(AuthorizationLevel.Function, "post")]
            HttpRequestMessage req, ExecutionContext context)
        {
            var body = await req.Content.ReadAsStringAsync();
            var events = JsonConvert.DeserializeObject<AuditRecord[]>(body);

            await Execute(events, context);

            return new HttpResponseMessage(HttpStatusCode.OK);
        }

        private async Task Execute(AuditRecord[] events, ExecutionContext context)
        {
            try
            {
                _logger.StartedTriggered("AuditLogEtl", "");

                await _consentEtl.Execute(events);
                await _loginEtl.Execute(events);
                await _webIntegrationReferralEtl.Execute(events);
            }
            catch (Exception)
            {
                var retryCount = context.RetryContext.RetryCount;
                var maxRetries = context.RetryContext.MaxRetryCount;

                if (retryCount >= maxRetries)
                {
                    _logger.Failed($"Encountered error while processing events. Max retries reached: {retryCount}");
                    _logger.Information("To replay these events reset the checkpoint to the first entry in range.");
                }
                else
                {
                    _logger.Failed($"Encountered error while processing events. Current retry count: {retryCount}.  Max retry count: {maxRetries}. Processing will be retried");
                }

                throw;
            }
        }
    }
}
