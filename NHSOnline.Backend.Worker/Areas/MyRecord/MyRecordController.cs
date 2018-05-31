using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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