using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;
using NHSOnline.Backend.Worker.TermsAndConditions;

namespace NHSOnline.Backend.Worker.Areas.TermsAndConditions
{
    [Route("patient/terms-and-conditions/consent"),PfsSecurityMode]
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
        public async Task<IActionResult> Post([FromBody] ConsentRequest model)
        {
            _logger.LogEnter();
            
            var userSession = HttpContext.GetUserSession();

            var termsAndConditionsAcceptanceDate = DateTimeOffset.Now;

            await AuditAttemptRecordConsent(model, termsAndConditionsAcceptanceDate);
            
            if (!model.AnalyticsCookieAccepted)
            {
                _logger.LogInformation($"Recording user did not accept optional analytics cookies. {nameof(userSession.GpUserSession.OdsCode)}: {userSession.GpUserSession.OdsCode}");
            }

            _logger.LogDebug("Recording user consent");         
            
            var recordConsentResult = await _termsAndConditionsService.RecordConsent(userSession.GpUserSession.NhsNumber, userSession.GpUserSession.OdsCode,
                model, termsAndConditionsAcceptanceDate);
         
            // Audit result of attempting to record consent
            recordConsentResult.Accept(new TermsAndConditionsRecordConsentAuditingVisitor(_auditor, model.ConsentGiven, termsAndConditionsAcceptanceDate,
                model.AnalyticsCookieAccepted, model.UpdatingConsent));
            
            _logger.LogExit();
            return recordConsentResult.Accept(new TermsAndConditionsRecordConsentResultVisitor());
        }

        private async Task AuditAttemptRecordConsent(ConsentRequest model, DateTimeOffset time)
        {
            var formattedDate = time.ToString(CultureInfo.InvariantCulture);

            await _auditor.Audit(Constants.AuditingTitles.TermsAndConditionsRecordConsentAuditTypeRequest,
                   $"Attempting to record patient consent - {nameof(model.ConsentGiven)}={model.ConsentGiven} " +
                   $"at DateOfConsent={formattedDate}");

            string cookieAcceptedLog = model.AnalyticsCookieAccepted ? $" at DateAnalyticsCookieAccepted={formattedDate}" : string.Empty;

            await _auditor.Audit(Constants.AuditingTitles.TermsAndConditionsAnalyticsCookieAcceptance,
                    $"Attempting to record analytics cookies acceptance - {nameof(model.AnalyticsCookieAccepted)}={model.AnalyticsCookieAccepted}" +
                    $"{cookieAcceptedLog}");
        }
    }
}