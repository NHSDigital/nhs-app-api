using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.TermsAndConditions
{
    [ProxyingNotAllowed]
    public class TermsAndConditionsController : Controller
    {
        private readonly ITermsAndConditionsService _termsAndConditionsService;
        private readonly ILogger<TermsAndConditionsController> _logger;
        private readonly IAuditor _auditor;

        public TermsAndConditionsController(
            ITermsAndConditionsService termsAndConditionsService,
            ILogger<TermsAndConditionsController> logger,
            IAuditor auditor)
        {
            _termsAndConditionsService = termsAndConditionsService;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpGet]
        [ApiVersionRoute("patient/terms-and-conditions/consent")]
        public async Task<IActionResult> Get()
        {
            _logger.LogEnter();

            var userSession = HttpContext.GetUserSession();

            _logger.LogDebug("Fetching user consent");
            var fetchConsentResult = await _termsAndConditionsService.FetchConsent(userSession.GpUserSession.NhsNumber);

            _logger.LogExit();
            return fetchConsentResult.Accept(new TermsAndConditionsFetchConsentResultVisitor());
        }

        [HttpPost]
        [ApiVersionRoute("patient/terms-and-conditions/consent")]
        public async Task<IActionResult> Post([FromBody] ConsentRequest model)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var userSession = HttpContext.GetUserSession();

            var termsAndConditionsAcceptanceDate = DateTimeOffset.Now;

            await _auditor.Audit(AuditingOperations.TermsAndConditionsRecordConsentAuditTypeRequest,
                $"Attempting to record patient consent - ConsentGiven={model.ConsentGiven}, " +
                $"AnalyticsCookieAccepted={model.AnalyticsCookieAccepted} " +
                $"at DateOfConsent={termsAndConditionsAcceptanceDate.ToString(CultureInfo.InvariantCulture)}");

            if (!model.AnalyticsCookieAccepted)
            {
                _logger.LogInformation(
                    $"Recording user did not accept optional analytics cookies." +
                    $" {nameof(userSession.GpUserSession.OdsCode)}: {userSession.GpUserSession.OdsCode}");
            }

            _logger.LogDebug("Recording user consent");

            var recordConsentResult = await _termsAndConditionsService.RecordConsent(
                userSession.GpUserSession.NhsNumber, userSession.GpUserSession.OdsCode,
                model, termsAndConditionsAcceptanceDate);

            await recordConsentResult.Accept(new TermsAndConditionsRecordConsentAuditingVisitor(
                _auditor, _logger, model, termsAndConditionsAcceptanceDate));

            _logger.LogExit();
            return recordConsentResult.Accept(new TermsAndConditionsRecordConsentResultVisitor());
        }

        [HttpPost]
        [ApiVersionRoute("patient/terms-and-conditions/toggle-analytics-cookie-acceptance")]
        public async Task<IActionResult> ToggleAnalyticsCookieAcceptance(
            [FromBody] AnalyticsCookieAcceptance analyticsCookieAcceptance)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var userSession = HttpContext.GetUserSession();
            var dateTimeOffset = DateTimeOffset.Now;

            await _auditor.Audit(AuditingOperations.TermsAndConditionsToggleAnalyticsCookieAcceptanceRequest,
                $"Attempting to toggle analytics cookie acceptance - " +
                $"AnalyticsCookieAccepted={analyticsCookieAcceptance.AnalyticsCookieAccepted} " +
                $"at DateAnalyticsCookieAccepted={dateTimeOffset.ToString(CultureInfo.InvariantCulture)}");

            var result = await _termsAndConditionsService.ToggleAnalyticsCookieAcceptance(
                userSession.GpUserSession.NhsNumber,
                analyticsCookieAcceptance, dateTimeOffset);

            await result.Accept(new ToggleAnalyticsCookieAcceptanceAuditingVisitor(_auditor, _logger,
                dateTimeOffset, analyticsCookieAcceptance.AnalyticsCookieAccepted));

            _logger.LogExit();

            return result.Accept(new ToggleAnalyticsCookieAcceptanceResultVisitor());
        }
    }
}