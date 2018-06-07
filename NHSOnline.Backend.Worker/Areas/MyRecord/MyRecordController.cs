using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/my-record")]
    public class MyRecordController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger _logger;
        
        public MyRecordController(
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

            return myRecordGetResult.Accept(new MyRecordResultVisitor());
        }
        
        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> GetPatientAllergies()
        {   
            UserSession userSession = HttpContext.GetUserSession();
            
            var patientRecordService = _bridgeFactory
                .CreateBridge(userSession.Supplier)
                .GetPatientRecordService();

            var result = await patientRecordService.GetPatientAllergies(userSession);

            return result.Accept(new AllergyResultVisitor());
        }
    }
}