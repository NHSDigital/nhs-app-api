using System;
using System.Threading.Tasks;
using System.Web.Http;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.WebJobs;
using Microsoft.Azure.WebJobs.Extensions.Http;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using NHSOnline.MetricLogFunctionApp.Monitoring.Models.AppInsightsAlert;

namespace NHSOnline.MetricLogFunctionApp.Monitoring
{
    internal sealed class MonitoringFunction
    {
        private readonly ILogger _logger;
        private readonly MonitoringService _monitoringService;

        public MonitoringFunction(ILogger<MonitoringFunction> logger, MonitoringService monitoringService)
        {
            _logger = logger;
            _monitoringService = monitoringService;
        }

        [FunctionName("Monitoring_AppInsightsAlert")]
        public async Task<IActionResult> AppInsightsAlert(
            [HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
        {
            _logger.LogEnter();
            try
            {
                var body = await req.ReadAsStringAsync();
                var content = JsonConvert.DeserializeObject<AppInsightAlert>(body);
                await _monitoringService.PostAlert(content);
                return new OkResult();
            }
            catch (Exception e)
            {
                _logger.LogMethodFailure(e);
                return new InternalServerErrorResult();
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}
