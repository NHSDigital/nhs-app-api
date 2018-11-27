using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;
using NHSOnline.Backend.Worker.Conventions;
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
            _logger.LogEnter(nameof(Get));
            
            var userSession = HttpContext.GetUserSession();
          
            _logger.LogDebug("Fetching user consent");
            var fetchConsentResult = await _termsAndConditionsService.FetchConsent(userSession.NhsNumber);
            
            _logger.LogExit(nameof(Get));
            return fetchConsentResult.Accept(new TermsAndConditionsFetchConsentResultVisitor());
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromBody] ConsentRequest model)
        {
            _logger.LogEnter(nameof(Post));
            
            var userSession = HttpContext.GetUserSession();

            var termsAndConditionsAcceptanceDate = DateTimeOffset.Now;

            // Audit attempt to record consent
            await _auditor.Audit(Constants.AuditingTitles.TermsAndConditionsRecordConsentAuditTypeRequest,
                "Attempting to record patient consent - ConsentGiven={0} at DateOfConsent={1:O}", model.ConsentGiven, termsAndConditionsAcceptanceDate);

            await _auditor.Audit(Constants.AuditingTitles.TermsAndConditionsAnalyticsCookieAcceptance,
                    "Attempting to record analytics cookies acceptance - AnalyticsCookieAccepted={0}{1}", model.AnalyticsCookieAccepted, 
                model.AnalyticsCookieAccepted ? 
                    string.Format(CultureInfo.InvariantCulture, " at DateAnalyticsCookieAccepted={0:O}", termsAndConditionsAcceptanceDate) 
                    : string.Empty);
            
            if (!model.AnalyticsCookieAccepted)
            {
                _logger.LogInformation("Recording user did not accept optional analytics cookies. OdsCode: {0}",
                    userSession.OdsCode);
            }

            _logger.LogDebug("Recording user consent");         
            
            var recordConsentResult = await _termsAndConditionsService.RecordConsent(userSession.NhsNumber, userSession.OdsCode,
                model, termsAndConditionsAcceptanceDate);
         
            // Audit result of attempting to record consent
            recordConsentResult.Accept(new TermsAndConditionsRecordConsentAuditingVisitor(_auditor, model.ConsentGiven, termsAndConditionsAcceptanceDate,
                model.AnalyticsCookieAccepted, model.UpdatingConsent));
            
            _logger.LogExit(nameof(Post));
            return recordConsentResult.Accept(new TermsAndConditionsRecordConsentResultVisitor());
        }
    }
}