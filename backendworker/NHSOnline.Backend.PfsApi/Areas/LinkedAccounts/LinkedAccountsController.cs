using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    [Route("patient/linked-accounts")]
    public class LinkedAccountsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<LinkedAccountsController> _logger;
        private readonly IGpSearchService _gpSearchService;
        private readonly ISessionCacheService _sessionCacheService;
        private readonly IAuditor _auditor;

        public LinkedAccountsController(
            ILogger<LinkedAccountsController> logger,
            IGpSystemFactory gpSystemFactory,
            IGpSearchService gpSearchService,
            ISessionCacheService sessionCacheService,
            IAuditor auditor)
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _gpSearchService = gpSearchService;
            _sessionCacheService = sessionCacheService;
            _auditor = auditor;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userSession = HttpContext.GetUserSession();

            await _auditor.Audit(AuditingOperations.GetLinkedAccountsRequest, "Retrieving linked accounts");
            _logger.LogInformation($"Fetching linked accounts supplier {userSession.GpUserSession.Supplier}");

            var linkedAccountsService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetLinkedAccountsService();

            var result = await linkedAccountsService.GetLinkedAccounts(userSession.GpUserSession);

            if (result is LinkedAccountsResult.Success success && success.HasAnyProxyInfoBeenUpdatedInSession)
            {
                _logger.LogInformation($"Updating user session to store Nhs Numbers for linked accounts");
                await _sessionCacheService.UpdateUserSession(userSession);
            }

            await result.Accept(new LinkedAccountsResultAuditingVisitor(_auditor, _logger));

            return await result.Accept(new LinkedAccountsResultVisitor());
        }

        [Route("access-summary")]
        [HttpGet]
        public async Task<IActionResult> GetAccessSummaryOfLinkedAccount([FromQuery] Guid id)
        {
            await _auditor.Audit(AuditingOperations.LinkedAccountsAccessSummaryRequest,
                $"Retrieving linked account summary detail for Id {id}");

            var auditResponse = AuditingOperations.LinkedAccountsAccessSummaryResponse;

            if (!ModelState.IsValid)
            {
                await _auditor.Audit(auditResponse, "ModelState is not valid.");
                return new BadRequestObjectResult(ModelState);
            }

            var userSession = HttpContext.GetUserSession();

            var linkedAccountsService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetLinkedAccountsService();

            _logger.LogDebug("Fetching linked account");

            var odsCodeForLinkedAccount =
                linkedAccountsService.GetOdsCodeForLinkedAccount(userSession.GpUserSession, id);

            _logger.LogDebug($"ODS code for linked account={odsCodeForLinkedAccount}");

            var linkedAccountSummaryTask = linkedAccountsService.GetLinkedAccount(userSession.GpUserSession, id);
            var gpPracticeSearchTask = _gpSearchService.GetGpPracticeByOdsCode(odsCodeForLinkedAccount);

            _logger.LogDebug("Running multiple tasks to get details for linked account");

            await Task.WhenAll(linkedAccountSummaryTask, gpPracticeSearchTask);

            if (!linkedAccountSummaryTask.IsCompletedSuccessfully)
            {
                var errorMessage = $"{nameof(linkedAccountSummaryTask)} did not complete successfully";
                _logger.LogError(errorMessage);
                await _auditor.Audit(auditResponse, errorMessage);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (linkedAccountSummaryTask.Result is LinkedAccountAccessSummaryResult.Success linkedAccountSuccess)
            {
                string gpPracticeName = GetPracticeNameToDisplay(gpPracticeSearchTask, odsCodeForLinkedAccount);

                _logger.LogInformation(
                    $"Has access to GP appointments: {linkedAccountSuccess.Response.CanBookAppointment}, Has access to repeat prescriptions: " +
                    $"{ linkedAccountSuccess.Response.CanOrderRepeatPrescription}, Has access to medical record: {linkedAccountSuccess.Response.CanViewMedicalRecord}");
                
                var response = new LinkedAccountAccessSummaryResponse
                {
                    CanBookAppointment = linkedAccountSuccess.Response.CanBookAppointment,
                    CanOrderRepeatPrescription = linkedAccountSuccess.Response.CanOrderRepeatPrescription,
                    CanViewMedicalRecord = linkedAccountSuccess.Response.CanViewMedicalRecord,
                    GpPracticeName = gpPracticeName,
                };

                await _auditor.Audit(auditResponse,
                    $"Successfully returned linked account summary detail for id {id}");
                return new OkObjectResult(response);
            }

            await _auditor.Audit(auditResponse,
                "Error retrieving linked account summary details: bad gateway");
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        [Route("switch/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> Switch(Guid id)
        {
            _logger.LogInformation($"Attempt to switch to profile id {id}");
            var userSession = HttpContext.GetUserSession();

            await _auditor.Audit(AuditingOperations.LinkedAccountsSwitchRequest,
                $"Request to switch to linked account {id}");

            var linkedAccountsService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetLinkedAccountsService();

            var IsValidAccountOrLinkedAccountId = linkedAccountsService.IsValidAccountOrLinkedAccountId(userSession.GpUserSession, id);

            if (IsValidAccountOrLinkedAccountId)
            {
                var linkedAccountAuditInfo = HttpContext.GetLinkedAccountAuditInfo();

                var fromNhsNumber = "";
                var toNhsNumber = "";
                if (linkedAccountAuditInfo.IsProxyMode)
                {
                    //switching from proxy to main account
                    fromNhsNumber = linkedAccountAuditInfo.ProxyNhsNumber;
                    toNhsNumber = userSession.GpUserSession.NhsNumber;
                }
                else
                {
                    //switching from main a/c
                    fromNhsNumber = userSession.GpUserSession.NhsNumber;
                    toNhsNumber = linkedAccountsService.GetNhsNumberForProxyUser(userSession.GpUserSession, id);
                }

                fromNhsNumber = fromNhsNumber.RemoveWhiteSpace();
                toNhsNumber = toNhsNumber.RemoveWhiteSpace();

                _logger.LogInformation($"Switching profile from nhsnumber={fromNhsNumber} to nhsnumber={toNhsNumber}");

                await _auditor.Audit(AuditingOperations.LinkedAccountsSwitchResponse,
                    $"Successfully switched profile to NhsNumber {toNhsNumber}");

                return Ok();
            }

            _logger.LogInformation($"Couldn't find profile with id {id} to switch to");
            await _auditor.Audit(AuditingOperations.LinkedAccountsSwitchResponse,
                $"Couldn't find profile with id {id} to switch to");
            return new NotFoundResult();
        }

        private string GetPracticeNameToDisplay(Task<GpSearchResult> gpSearchTask, string odsCodeSearched)
        {
            string gpPracticeName = string.Empty;

            if (gpSearchTask.IsCompletedSuccessfully &&
                gpSearchTask.Result is GpSearchResult.Success gpPracticeSearchSuccess)
            {
                if (gpPracticeSearchSuccess.Response.Organisations.Count == 1)
                {
                    gpPracticeName = gpPracticeSearchSuccess.Response.Organisations.First().OrganisationName;
                }
                else
                {
                    gpPracticeName = odsCodeSearched;
                    _logger.LogWarning(
                        $"{gpPracticeSearchSuccess.Response.Organisations.Count} search results for ODS code {odsCodeSearched}." +
                        " Using ODS code for practice name.");
                }
            }
            else
            {
                _logger.LogError("GP search did not complete successfully. Practice name will not be set.");
            }

            return gpPracticeName;
        }
    }
}
