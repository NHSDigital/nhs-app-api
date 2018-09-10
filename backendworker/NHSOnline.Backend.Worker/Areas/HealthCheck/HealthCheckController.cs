using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.HealthCheck;

namespace NHSOnline.Backend.Worker.Areas.HealthCheck
{
    [Route("patient/healthcheck"),PfsSecurityMode]
    public class HealthCheckController : Controller
    {
        private readonly IHealthCheckService _healthCheckService;
        
        public HealthCheckController(IHealthCheckService healthCheckService)
        {
            _healthCheckService = healthCheckService;
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> RunHealthCheck()
        {
            var healthCheckResult = await _healthCheckService.RunHealthChecks();
            return new OkObjectResult(healthCheckResult);
        }
    }
}