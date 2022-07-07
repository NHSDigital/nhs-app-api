using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.MetricLogging
{
    public class NativeMetricsController : Controller
    {
        private readonly ILogger<NativeMetricsController> _logger;
        private readonly IAuditor _auditor;

        public NativeMetricsController(ILogger<NativeMetricsController> logger,
            IAuditor auditor)
        {
            _logger = logger;
            _auditor = auditor;

        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [ApiVersionRoute("api/metrics/biometrics/optIn")]
        public async Task<IActionResult> PostBiometricsOptInMetrics()
        {
            try
            {
                _logger.LogEnter();

                var auditResponse = AuditingOperations.BiometricsOptInDecision;
                await _auditor.PostOperationAudit(auditResponse, $"Biometrics toggled. optIn={true}");
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpPost]
        [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
        [ApiVersionRoute("api/metrics/biometrics/optOut")]
        public async Task<IActionResult> PostBiometricsOptOutMetrics()
        {
            try
            {
                _logger.LogEnter();

                var auditResponse = AuditingOperations.BiometricsOptInDecision;
                await _auditor.PostOperationAudit(auditResponse, $"Biometrics toggled. optIn={false}");
                return new StatusCodeResult(StatusCodes.Status200OK);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}