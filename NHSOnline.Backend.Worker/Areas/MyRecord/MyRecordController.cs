using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;
using NHSOnline.Backend.Worker.Router.Validators;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/myrecord")]
    public class MyRecordController : Controller
    {
        private readonly ConfigurationSettings _settings;
        private readonly IBridgeFactory _bridgeFactory;
        private readonly ILogger<MyRecordController> _logger;

        public MyRecordController(
            IOptions<ConfigurationSettings> settings,
            ILoggerFactory loggerFactory,
            IBridgeFactory bridgeFactory)
        {
            _settings = settings.Value;
            _logger = loggerFactory.CreateLogger<MyRecordController>();
            _bridgeFactory = bridgeFactory;
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

using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Router;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/my-record")]
    public class MyRecordController : Controller
    {
        private readonly IBridgeFactory _bridgeFactory;
        private readonly ILogger _logger;
        
        public MyRecordController(
            ILoggerFactory loggerFactory,
            IBridgeFactory bridgeFactory)
        {
            _bridgeFactory = bridgeFactory;
            _logger = loggerFactory.CreateLogger<MyRecordController>();
        }

        [HttpGet("demographics")]
        [TimeoutExceptionFilter]
        public async Task<IActionResult> Get()
        {
            var userSession = HttpContext.GetUserSession();

            var demographicsService = _bridgeFactory
                .CreateBridge(userSession.Supplier)
                .GetDemographicsService();

            _logger.LogDebug("Fetching Demographics");

            var myRecordGetResult = await demographicsService.Get(userSession);

            return myRecordGetResult.Accept(new MyRecordResultVisitor());
        }
    }
}