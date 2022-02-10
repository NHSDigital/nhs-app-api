using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.MyRecord
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)]
    public class HistoricTestResultsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<HistoricTestResultsController> _logger;
        private readonly IAuditor _auditor;

        public HistoricTestResultsController(
            ILogger<HistoricTestResultsController> logger,
            IGpSystemFactory gpSystemFactory,
            IAuditor auditor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpGet]
        [ApiVersionRoute("patient/historic-test-results/{year}")]
        public async Task<IActionResult> GetHistoricTestResults(
            [FromRoute(Name = "year")] string year,
            [FromHeader(Name=PatientId)] Guid patientId,
            [UserSession] P9UserSession userSession,
            [GpSession] GpUserSession gpUserSession)
        {
            _logger.LogEnter();

            _logger.LogDebug($"{nameof(GetHistoricTestResults)} with patientId {patientId}");

            if (int.TryParse(year, out var yearAsInt) == false)
            {
                return BadRequest($"A valid year was not supplied to {nameof(GetHistoricTestResults)}");
            }

            var gpSystem = gpUserSession.Supplier;
            if (gpSystem != Supplier.Tpp)
            {
                return BadRequest($"The {nameof(GetHistoricTestResults)} endpoint only works with TPP");
            }

            // Audit attempt made to view past year's worth of records
            await _auditor.PreOperationAudit(AuditingOperations.GetHistoricTestResultsAuditTypeRequest, $"Viewing Historic Test Results for year {year}");

            var patientRecordService = _gpSystemFactory.CreateGpSystem(gpSystem).GetPatientRecordService();
            var gpLinkedAccountUserSession = userSession.BuildGpLinkedAccountModel(patientId);

            _logger.LogInformation($"Fetching historic test results for year {year}");
            var result = await patientRecordService.GetHistoricTestResults(gpLinkedAccountUserSession, yearAsInt);

            // Audit result of attempt to view patient record
            await result.Accept(new HistoricTestResultsAuditingVisitor(_auditor, _logger, yearAsInt));

            _logger.LogExit();
            return result.Accept(new HistoricTestResultsVisitor());
        }
    }
}
