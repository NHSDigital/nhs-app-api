using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.GpSystems.Demographics;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    [Route("patient"),PfsSecurityMode]
    public class DemographicsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<DemographicsController> _logger;
        private readonly IDemographicsResultVisitor<IActionResult> _demographicsResultVisitor;
        
        public DemographicsController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory,
            IDemographicsResultVisitor<IActionResult> demographicsResultVisitor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = loggerFactory.CreateLogger<DemographicsController>();
            _demographicsResultVisitor = demographicsResultVisitor;
        }

        [HttpGet("demographics")]
        public async Task<IActionResult> Get()
        {
            _logger.LogEnter();
            var userSession = HttpContext.GetUserSession();

            _logger.LogInformation($"Fetching DemographicsService for supplier: {userSession.GpUserSession.Supplier}");
            var demographicsService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetDemographicsService();

            _logger.LogDebug("Fetching Demographics");
            var myRecordGetResult = await demographicsService.GetDemographics(userSession);

            _logger.LogExit();
            return myRecordGetResult.Accept(_demographicsResultVisitor);
        }
    }
}