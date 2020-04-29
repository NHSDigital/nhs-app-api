using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.PfsApi.TermsAndConditions;
using NHSOnline.Backend.PfsApi.TermsAndConditions.Models;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;

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
        public async Task<IActionResult> Get([UserSession] P5UserSession userSession)
        {
            _logger.LogEnter();

            var nhsLoginId = GetNhsLoginId(userSession);

            _logger.LogDebug("Fetching user consent");
            var fetchConsentResult = await _termsAndConditionsService.FetchConsent(nhsLoginId);

            _logger.LogExit();
            return fetchConsentResult.Accept(new TermsAndConditionsFetchConsentResultVisitor());
        }

        [HttpPost]
        [ApiVersionRoute("patient/terms-and-conditions/consent")]
        public async Task<IActionResult> Post(
            [FromBody] ConsentRequest model,
            [UserSession] P5UserSession userSession)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var termsAndConditionsAcceptanceDate = DateTimeOffset.Now;

            var auditingVisitor = new TermsAndConditionsRecordConsentAuditingVisitor(
                _auditor,
                model,
                termsAndConditionsAcceptanceDate,
               async () => await RecordConsent(model, userSession, termsAndConditionsAcceptanceDate));
            var recordConsentResult = await userSession.Accept(auditingVisitor);

            _logger.LogExit();
            return recordConsentResult.Accept(new TermsAndConditionsRecordConsentResultVisitor());
        }

        private async Task<TermsAndConditionsRecordConsentResult> RecordConsent(
            ConsentRequest model,
            P5UserSession userSession,
            DateTimeOffset termsAndConditionsAcceptanceDate)
        {
            if (!model.AnalyticsCookieAccepted)
            {
                _logger.LogInformation(
                    $"Recording user did not accept optional analytics cookies." +
                    $" {nameof(userSession.OdsCode)}: {userSession.OdsCode}");
            }

            _logger.LogDebug("Recording user consent");

            var nhsLoginId = GetNhsLoginId(userSession);

            return await _termsAndConditionsService.RecordConsent(nhsLoginId, model, termsAndConditionsAcceptanceDate);
        }

        [HttpPost]
        [ApiVersionRoute("patient/terms-and-conditions/toggle-analytics-cookie-acceptance")]
        public async Task<IActionResult> ToggleAnalyticsCookieAcceptance(
            [FromBody] AnalyticsCookieAcceptance analyticsCookieAcceptance,
            [UserSession] P5UserSession userSession)
        {
            _logger.LogEnter();

            if (!ModelState.IsValid)
            {
                return new BadRequestObjectResult(ModelState);
            }

            var nhsLoginId = GetNhsLoginId(userSession);
            var dateTimeOffset = DateTimeOffset.Now;

            var auditingVisitor = new ToggleAnalyticsCookieAcceptanceAuditingVisitor(
                _auditor,
                dateTimeOffset,
                analyticsCookieAcceptance.AnalyticsCookieAccepted,
                async () => await _termsAndConditionsService.ToggleAnalyticsCookieAcceptance(nhsLoginId, analyticsCookieAcceptance, dateTimeOffset));
            var result = await userSession.Accept(auditingVisitor);

            _logger.LogExit();

            return result.Accept(new ToggleAnalyticsCookieAcceptanceResultVisitor());
        }

        private string GetNhsLoginId(P5UserSession userSession)
            => AccessToken.Parse(_logger, userSession.CitizenIdUserSession.AccessToken).Subject;
    }
}
