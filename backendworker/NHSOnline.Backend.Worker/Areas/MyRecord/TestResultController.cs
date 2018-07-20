using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.GpSystems;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/test-result")]
    public class TestResultController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger _logger;
        
        public TestResultController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = loggerFactory.CreateLogger<TestResultController>();
        }

        [HttpGet]
        [TimeoutExceptionFilter]
        public async Task<IActionResult> GetTestResult([FromQuery] string testResultId)
        {   
            var userSession = HttpContext.GetUserSession();
            
            var patientRecordService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetPatientRecordService();

            var result = await patientRecordService.GetDetailedTestResult(userSession, testResultId);
            
            return result.Accept(new TestResultVisitor());
        }
    }
}