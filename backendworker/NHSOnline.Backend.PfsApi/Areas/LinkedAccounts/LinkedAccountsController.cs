using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.Areas.LinkedAccounts.Models;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
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

        [ApiVersionRoute("patient/linked-accounts")]
        [HttpGet]
        public async Task<IActionResult> Get(
            [GpSession] GpUserSession gpUserSession,
            [UserSession] P9UserSession userSession)
        {
            await _auditor.PreOperationAudit(AuditingOperations.GetLinkedAccountsRequest, "Retrieving linked accounts");
            _logger.LogInformation($"Fetching linked accounts supplier {gpUserSession.Supplier}");

            var linkedAccountsService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetLinkedAccountsService();

            var result = await linkedAccountsService.GetLinkedAccounts(gpUserSession);

            if (result is LinkedAccountsResult.Success success && success.HasAnyProxyInfoBeenUpdatedInSession)
            {
                _logger.LogInformation("Updating user session to store Nhs Numbers for linked accounts");
                await _sessionCacheService.UpdateUserSession(userSession);
            }

            await result.Accept(new LinkedAccountsResultAuditingVisitor(_auditor, _logger));

            return await result.Accept(new LinkedAccountsResultVisitor());
        }

        [ApiVersionRoute("patient/linked-accounts/access-summary")]
        [HttpGet]
        public async Task<IActionResult> GetAccessSummaryOfLinkedAccount([FromQuery] Guid id,
            [GpSession] GpUserSession gpUserSession)
        {
            await _auditor.PreOperationAudit(AuditingOperations.LinkedAccountsAccessSummaryRequest,
                $"Retrieving linked account summary detail for Id {id}");

            var auditResponse = AuditingOperations.LinkedAccountsAccessSummaryResponse;

            if (!ModelState.IsValid)
            {
                await _auditor.PostOperationAudit(auditResponse, "ModelState is not valid.");
                return new BadRequestObjectResult(ModelState);
            }

            var linkedAccountsService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetLinkedAccountsService();

            _logger.LogDebug("Fetching linked account");

            var odsCodeForLinkedAccount =
                linkedAccountsService.GetOdsCodeForLinkedAccount(gpUserSession, id);

            _logger.LogDebug($"ODS code for linked account={odsCodeForLinkedAccount}");

            var linkedAccountSummaryTask = linkedAccountsService.GetLinkedAccount(gpUserSession, id);
            var gpPracticeSearchTask = _gpSearchService.GetGpPracticeByOdsCode(odsCodeForLinkedAccount);

            _logger.LogDebug("Running multiple tasks to get details for linked account");

            await Task.WhenAll(linkedAccountSummaryTask, gpPracticeSearchTask);

            if (!linkedAccountSummaryTask.IsCompletedSuccessfully)
            {
                var errorMessage = $"{nameof(linkedAccountSummaryTask)} did not complete successfully";
                _logger.LogError(errorMessage);
                await _auditor.PostOperationAudit(auditResponse, errorMessage);
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (linkedAccountSummaryTask.Result is LinkedAccountAccessSummaryResult.Success linkedAccountSuccess)
            {
                string gpPracticeName = GetPracticeNameToDisplay(gpPracticeSearchTask, odsCodeForLinkedAccount);

                _logger.LogInformation(
                    $"Has access to GP appointments: {linkedAccountSuccess.Response.CanBookAppointment}, Has access to repeat prescriptions: " +
                    $"{ linkedAccountSuccess.Response.CanOrderRepeatPrescription}, Has access to medical record: {linkedAccountSuccess.Response.CanViewMedicalRecord}");

                var accountSuccessResponse = linkedAccountSuccess.Response;
                var response = new LinkedAccountAccessSummaryResponse
                {
                    ShowSummary = accountSuccessResponse.IsValidData,
                    CanBookAppointment = accountSuccessResponse.CanBookAppointment,
                    CanOrderRepeatPrescription = accountSuccessResponse.CanOrderRepeatPrescription,
                    CanViewMedicalRecord = accountSuccessResponse.CanViewMedicalRecord,
                    GpPracticeName = gpPracticeName,
                };

                await _auditor.PostOperationAudit(auditResponse,
                    $"Successfully returned linked account summary detail for id {id}");
                return new OkObjectResult(response);
            }

            await _auditor.PostOperationAudit(auditResponse,
                "Error retrieving linked account summary details: bad gateway");
            return new StatusCodeResult(StatusCodes.Status502BadGateway);
        }

        [ApiVersionRoute("patient/linked-accounts/switch/{id:guid}")]
        [HttpPost]
        public async Task<IActionResult> Switch(Guid id, [GpSession] GpUserSession gpUserSession)
        {
            _logger.LogInformation($"Attempt to switch to profile id {id}");

            await _auditor.PreOperationAudit(AuditingOperations.LinkedAccountsSwitchRequest,
                $"Request to switch to linked account {id}");

            var linkedAccountsService = _gpSystemFactory
                .CreateGpSystem(gpUserSession.Supplier)
                .GetLinkedAccountsService();

            var linkedAccountModel = new GpLinkedAccountModel(gpUserSession, id);

            var switchResult = await linkedAccountsService.SwitchAccount(linkedAccountModel);

            if (switchResult is SwitchAccountResult.Success success)
            {
                var linkedAccountAuditInfo = HttpContext.GetLinkedAccountAuditInfo();

                var (fromNhsNumber, toNhsNumber) = GetNhsNumbers(id, linkedAccountAuditInfo, gpUserSession, linkedAccountsService);
                success.ToNhsNumber = toNhsNumber;

                _logger.LogInformation($"Switching profile from nhsnumber={fromNhsNumber} to nhsnumber={toNhsNumber}");
            }

            await switchResult.Accept(new SwitchAccountResultAuditingVisitor(_auditor, _logger));
            return await switchResult.Accept(new SwitchAccountResultVisitor());
        }

        private static (string fromNhsNumber, string toNhsNumber) GetNhsNumbers(Guid id,
            LinkedAccountAuditInfo linkedAccountAuditInfo, GpUserSession gpUserSession,
            ILinkedAccountsService linkedAccountsService)
        {
            var fromNhsNumber = "";
            var toNhsNumber = "";
            if (linkedAccountAuditInfo.IsProxyMode)
            {
                //switching from proxy to main account
                fromNhsNumber = linkedAccountAuditInfo.ProxyNhsNumber;
                toNhsNumber = gpUserSession.NhsNumber;
            }
            else
            {
                //switching from main a/c
                fromNhsNumber = gpUserSession.NhsNumber;
                toNhsNumber = linkedAccountsService.GetNhsNumberForProxyUser(gpUserSession, id);
            }

            fromNhsNumber = fromNhsNumber.RemoveWhiteSpace();
            toNhsNumber = toNhsNumber.RemoveWhiteSpace();
            return (fromNhsNumber, toNhsNumber);
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
