using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Areas.TermsAndConditions.Models;
using NHSOnline.Backend.Worker.Filters;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.TermsAndConditions;

namespace NHSOnline.Backend.Worker.Areas.TermsAndConditions
{
    [Route("patient/terms-and-conditions/consent")]
    public class TermsAndConditionsController : Controller
    {
        private readonly ITermsAndConditionsService _termsAndConditionsService;
        private readonly ILogger _logger;
        private readonly IAuditor _auditor;        
        
        public TermsAndConditionsController(
            ITermsAndConditionsService termsAndConditionsService,
            ILoggerFactory loggerFactory,
            IAuditor auditor)
        {
            _termsAndConditionsService = termsAndConditionsService;
            _logger = loggerFactory.CreateLogger<TermsAndConditionsController>();
            _auditor = auditor;
        }

        [HttpGet, TimeoutExceptionFilter]
        public async Task<IActionResult> Get()
        {
            var methodName = "Get";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var userSession = HttpContext.GetUserSession();
          
            _logger.LogDebug("Fetching user consent");
            var fetchConsentResult = await _termsAndConditionsService.FetchConsent(userSession.NhsNumber);
            
            _logger.LogDebug("Exiting: {0}", methodName);
            return fetchConsentResult.Accept(new TermsAndConditionsFetchConsentResultVisitor());
        }

        [HttpPost, TimeoutExceptionFilter]
        public async Task<IActionResult> Post([FromBody] ConsentRequest model)
        {
            var methodName = "Post";
            _logger.LogDebug("Entered: {0}", methodName);
            
            var userSession = HttpContext.GetUserSession();
        
            // Audit attempt to record consent
            _auditor.Audit(Constants.AuditingTitles.TermsAndConditionsRecordConsentAuditTypeRequest,
                "Attempting to record patient consent - ConsentGiven={0} at DateOfConsent={1:O}", model.ConsentGiven, model.DateOfConsent);
          
            _logger.LogDebug("Recording user consent");
            var recordConsentResult = await _termsAndConditionsService.RecordConsent(userSession.NhsNumber, model);
            
            // Audit result of attempting to record consent
            recordConsentResult.Accept(new TermsAndConditionsRecordConsentAuditingVisitor(_auditor, model.ConsentGiven, model.DateOfConsent));
            
            _logger.LogDebug("Exiting: {0}", methodName);
            return recordConsentResult.Accept(new TermsAndConditionsRecordConsentResultVisitor());
        }
    }
}