using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.MyRecord;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker.Areas.Demographics
{
    [Route("patient")]
    public class DemographicsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger _logger;
        
        public DemographicsController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = loggerFactory.CreateLogger<MyRecordController>();
        }

        [HttpGet("demographics")]
        [TimeoutExceptionFilter]
        public async Task<IActionResult> Get()
        {
            var userSession = HttpContext.GetUserSession();

            var demographicsService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetDemographicsService();

            _logger.LogDebug("Fetching Demographics");

            var myRecordGetResult = await demographicsService.Get(userSession);

            return myRecordGetResult.Accept(new DemographicsResultVisitor());
        }
    }
}