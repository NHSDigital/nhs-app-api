using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.PfsApi.GpSearch;
using NHSOnline.Backend.PfsApi.GpSearch.Models;
using NHSOnline.Backend.Support.AspNet;

namespace NHSOnline.Backend.PfsApi.Areas.LinkedAccounts
{
    [Route("patient/linked-accounts")]
    public class LinkedAccountsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<LinkedAccountsController> _logger;
        private readonly IGpSearchService _gpSearchService;

        public LinkedAccountsController(
            ILogger<LinkedAccountsController> logger,
            IGpSystemFactory gpSystemFactory,
            IGpSearchService gpSearchService)
        {
            _logger = logger;
            _gpSystemFactory = gpSystemFactory;
            _gpSearchService = gpSearchService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            var userSession = HttpContext.GetUserSession();

            _logger.LogInformation($"Fetching linked accounts supplier {userSession.GpUserSession.Supplier}");

            var linkedAccountsService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetLinkedAccountsService();

            var result = await linkedAccountsService.GetLinkedAccounts(userSession.GpUserSession);

            return await result.Accept(new LinkedAccountsResultVisitor());
        }

        [Route("access-summary")]
        [HttpGet]
        public async Task<IActionResult> GetAccessSummaryOfLinkedAccount([FromQuery] Guid id)
        {
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
                _logger.LogError($"{nameof(linkedAccountSummaryTask)} did not complete successfully");
                return new StatusCodeResult(StatusCodes.Status500InternalServerError);
            }

            if (linkedAccountSummaryTask.Result is LinkedAccountAccessSummaryResult.Success linkedAccountSuccess)
            {
                string gpPracticeName = GetPracticeNameToDisplay(gpPracticeSearchTask, odsCodeForLinkedAccount);

                var response = new LinkedAccountAccessSummaryResponse
                {
                    CanBookAppointment = linkedAccountSuccess.Response.CanBookAppointment,
                    CanOrderRepeatPrescription = linkedAccountSuccess.Response.CanOrderRepeatPrescription,
                    CanViewMedicalRecord = linkedAccountSuccess.Response.CanViewMedicalRecord,
                    GpPracticeName = gpPracticeName,
                };

                return new OkObjectResult(response);
            }

            return new StatusCodeResult(StatusCodes.Status502BadGateway);
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
