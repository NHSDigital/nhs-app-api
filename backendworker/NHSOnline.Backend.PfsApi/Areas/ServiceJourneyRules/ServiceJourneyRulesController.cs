using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.PfsApi.Areas.LinkedAccounts;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.ServiceJourneyRules
{
    public class ServiceJourneyRulesController : Controller
    {
        private readonly ILogger<ServiceJourneyRulesController> _logger;
        private readonly IAuditor _auditor;
        private readonly IServiceJourneyRulesService _serviceJourneyRulesService;
        private readonly SessionConfigurationSettings _sessionSettings;

        public ServiceJourneyRulesController(
            ILogger<ServiceJourneyRulesController> logger,
            IAuditor auditor, 
            IServiceJourneyRulesService serviceJourneyRulesService,
            SessionConfigurationSettings sessionSettings
        )
        {
            _logger = logger;
            _auditor = auditor;
            _serviceJourneyRulesService = serviceJourneyRulesService;
            _sessionSettings = sessionSettings;
        }

        [HttpGet]
        [Route("patient/journey-configuration")]
        public async Task<IActionResult> Get()
        {
            try
            {
                _logger.LogEnter();
                
                await _auditor.Audit(AuditingOperations.GetServiceJourneyRulesAuditTypeRequest,
                    "Attempting to get service journey rules");

                var odsCode = HttpContext.GetUserSession().GpUserSession.OdsCode;

                _logger.LogInformation($"Fetching Service Journey Rules for {odsCode}");
                
                var result = await _serviceJourneyRulesService.GetServiceJourneyRulesForOdsCode(odsCode);

                return result.Accept(new ServiceJourneyRulesGetResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Route("patient/configuration")]
        public async Task<IActionResult> GetLinkedAccountPatientConfig()
        {
            await _auditor.Audit(AuditingOperations.GetPatientGuid,"Attempting to get Guid for patient");

            var userSession = HttpContext.GetUserSession();
            var enableLinkedAccounts = _sessionSettings.ProxyEnabled && userSession.GpUserSession.HasLinkedAccounts;

            LinkedAccountsConfigResult result = new LinkedAccountsConfigResult.Success(new LinkedAccountsConfigResponse
            {
                Id = userSession.GpUserSession.Id,
                HasLinkedAccounts = enableLinkedAccounts
            });
            
            return result.Accept(new LinkedAccountsConfigResultVisitor());
        }
        
    }
}