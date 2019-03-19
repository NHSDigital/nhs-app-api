using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.PfsApi.HealthCheck;

namespace NHSOnline.Backend.PfsApi.Areas.HealthCheck
{
    [Route("healthcheck")]
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