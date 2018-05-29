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
