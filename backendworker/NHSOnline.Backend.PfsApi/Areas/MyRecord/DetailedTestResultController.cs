using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    [ApiVersionRoute("patient/test-result")]
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
        public async Task<IActionResult> GetTestResult(
            [FromHeader(Name=PatientId)] Guid patientId,
            [FromQuery] string testResultId,
            [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                await _auditor.PreOperationAudit(AuditingOperations.GetTestResultAuditTypeRequest,
                    "Attempting to view test result");

                _logger.LogInformation($"Fetching PatientRecordService for supplier: {userSession.GpUserSession}");
                var patientRecordService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetPatientRecordService();

                var gpLinkedAccountUserSession = new GpLinkedAccountModel(
                    userSession.GpUserSession, patientId
                );

                _logger.LogInformation("Fetching detailed test result");
                var result = await patientRecordService.GetDetailedTestResult(gpLinkedAccountUserSession, testResultId);

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
