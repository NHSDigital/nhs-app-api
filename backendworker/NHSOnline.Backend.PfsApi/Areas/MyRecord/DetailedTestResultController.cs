using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    [Route("patient/test-result")]
    public class DetailedTestResultController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<DetailedTestResultController> _logger;
        private readonly IAuditor _auditor;
        
        public DetailedTestResultController(
            ILogger<DetailedTestResultController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> GetTestResult([FromQuery] string testResultId)
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(AuditingOperations.GetTestResultAuditTypeRequest,
                    "Attempting to view test result");

                var userSession = HttpContext.GetUserSession();

                _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession}");
                var patientRecordService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetPatientRecordService();

                _logger.LogInformation("Fetching detailed test result");
                var result = await patientRecordService.GetDetailedTestResult(userSession.GpUserSession, testResultId);

                await result.Accept(new DetailedTestResultAuditingVisitor(_auditor, _logger));
                return result.Accept(new DetailedTestResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}