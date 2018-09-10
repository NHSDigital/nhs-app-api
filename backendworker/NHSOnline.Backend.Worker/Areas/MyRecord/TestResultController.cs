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
        public async Task<IActionResult> GetTestResult([FromQuery] string testResultId)
        {   
            var methodName = "GetTestResult";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var userSession = HttpContext.GetUserSession();
            
            _logger.LogInformation("Fetching PatientRecordService for supplier: {0}", userSession.Supplier.ToString());  
            var patientRecordService = _gpSystemFactory
                .CreateGpSystem(userSession.Supplier)
                .GetPatientRecordService();

            _logger.LogInformation("Fetching detailed test result");
            var result = await patientRecordService.GetDetailedTestResult(userSession, testResultId);
            
            _logger.LogDebug("Exiting: {0}", methodName);
            return result.Accept(new TestResultVisitor());
        }
    }
}