using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    [Route("patient"),PfsSecurityMode]
    public class DemographicsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<DemographicsController> _logger;
        private readonly IDemographicsResultVisitor<IActionResult> _demographicsResultVisitor;
        private readonly IAuditor _auditor;

        public DemographicsController(
            ILogger<DemographicsController> logger,
            IGpSystemFactory gpSystemFactory,
            IDemographicsResultVisitor<IActionResult> demographicsResultVisitor,
            IAuditor auditor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _demographicsResultVisitor = demographicsResultVisitor;
            _auditor = auditor;
        }

        [HttpGet("demographics")]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();

                await _auditor.Audit(Constants.AuditingTitles.GetDemographicsAuditTypeRequest,
                    "Attempting to view Demographics");
                
                var userSession = HttpContext.GetUserSession();

                _logger.LogDebug($"Fetching DemographicsService for supplier: {userSession.GpUserSession.Supplier}");
                var demographicsService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetDemographicsService();

                _logger.LogDebug("Fetching Demographics");
                var result = await demographicsService.GetDemographics(userSession);

                await result.Accept(new DemographicsAuditingVisitor(_auditor, _logger));
                
                return result.Accept(_demographicsResultVisitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}