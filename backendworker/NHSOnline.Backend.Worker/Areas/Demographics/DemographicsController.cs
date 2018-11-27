using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    [Route("patient"),PfsSecurityMode]
    public class DemographicsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<DemographicsController> _logger;
        
        public DemographicsController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = loggerFactory.CreateLogger<DemographicsController>();
        }

        [HttpGet("demographics")]
        public async Task<IActionResult> Get()
        {
            _logger.LogEnter();
            var userSession = HttpContext.GetUserSession();

            _logger.LogInformation("Fetching DemographicsService for supplier: {0}", userSession.Supplier.ToString());
            var demographicsService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetDemographicsService();

            _logger.LogDebug("Fetching Demographics");
            var myRecordGetResult = await demographicsService.GetDemographics(userSession);

            _logger.LogExit();
            return myRecordGetResult.Accept(new DemographicsResultVisitor());
        }
    }
}