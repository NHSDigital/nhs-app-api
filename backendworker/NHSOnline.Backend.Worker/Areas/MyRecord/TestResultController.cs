using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.MyRecord
{
    [Route("patient/test-result"),PfsSecurityMode]
    public class TestResultController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<TestResultController> _logger;
        private readonly IAuditor _auditor;
        
        public TestResultController(
            ILoggerFactory loggerFactory,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor
        )
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = loggerFactory.CreateLogger<TestResultController>();
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> GetTestResult([FromQuery] string testResultId)
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(Constants.AuditingTitles.GetTestResultAuditTypeRequest,
                    "Attempting to view test result");

                var userSession = HttpContext.GetUserSession();

                _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession}");
                var patientRecordService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetPatientRecordService();

                _logger.LogInformation("Fetching detailed test result");
                var result = await patientRecordService.GetDetailedTestResult(userSession, testResultId);

                result.Accept(new TestResultAuditingVisitor(_auditor));
                return result.Accept(new TestResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}