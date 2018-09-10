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
        public async Task<IActionResult> Get()
        {
            var methodName = "Get";
            _logger.LogDebug("Entered: {0}", methodName);
            var userSession = HttpContext.GetUserSession();

            _logger.LogInformation("Fetching DemographicsService for supplier: {0}", userSession.Supplier.ToString());
            var demographicsService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetDemographicsService();

            _logger.LogDebug("Fetching Demographics");
            var myRecordGetResult = await demographicsService.GetDemographics(userSession);

            _logger.LogDebug("Exiting: {0}", methodName);
            return myRecordGetResult.Accept(new DemographicsResultVisitor());
        }
        
        /// <summary>
        /// Remove this method after bddtests that use above route are committed.  The build pipeline will fail if
        /// this is missing because current bddtests refer to this route.
        /// </summary>
        /// <returns></returns>
        [HttpGet("my-record/demographics")]
        public async Task<IActionResult> GetOld()
        {
            var userSession = HttpContext.GetUserSession();

            var demographicsService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetDemographicsService();

            _logger.LogDebug("Fetching Demographics");

            var myRecordGetResult = await demographicsService.GetDemographics(userSession);

            return myRecordGetResult.Accept(new DemographicsResultVisitor());
        }
    }
}