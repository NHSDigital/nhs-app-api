using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceJourneyRules
{
    [Route("patient/journey-configuration")]
    public class ServiceJourneyRulesController : Controller
    {
        private readonly ILogger<ServiceJourneyRulesController> _logger;
        private readonly IAuditor _auditor;
        private readonly IServiceJourneyRulesService _serviceJourneyRulesService;

        public ServiceJourneyRulesController(
            ILogger<ServiceJourneyRulesController> logger,
            IAuditor auditor, 
            IServiceJourneyRulesService serviceJourneyRulesService
        )
        {
            _logger = logger;
            _auditor = auditor;
            _serviceJourneyRulesService = serviceJourneyRulesService;
        }

        [HttpGet]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(Constants.AuditingTitles.GetServiceJourneyRulesAuditTypeRequest,
                    "Attempting to get service journey rules");

                var userSession = HttpContext.GetUserSession();
                var odsCode = userSession.GpUserSession.OdsCode;

                _logger.LogInformation($"Fetching Service Journey Rules for {userSession.GpUserSession.OdsCode}");
                
                var result = await _serviceJourneyRulesService.GetServiceJourneyRulesForOdsCode(odsCode);

                return result.Accept(new ServiceJourneyRulesGetResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}