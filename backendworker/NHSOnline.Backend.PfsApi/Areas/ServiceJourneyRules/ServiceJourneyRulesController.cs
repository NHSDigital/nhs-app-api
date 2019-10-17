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

                var userSession = HttpContext.GetUserSession();
                var odsCode = userSession.GpUserSession.OdsCode;
                var enableLinkedAccounts = _sessionSettings.ProxyEnabled && userSession.GpUserSession.HasLinkedAccounts;

                _logger.LogInformation($"Fetching Service Journey Rules for {userSession.GpUserSession.OdsCode}");
                
                var result = await _serviceJourneyRulesService.GetServiceJourneyRulesForOdsCode(odsCode, enableLinkedAccounts);

                return result.Accept(new ServiceJourneyRulesGetResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

        [HttpGet]
        [Route("patient/configuration")]
        public async Task<IActionResult> GetPatientGuid()
        {
            await _auditor.Audit(AuditingOperations.GetPatientGuid,"Attempting to get Guid for patient");

            GetPatientGuidResult result = new GetPatientGuidResult.Success(new PatientIdResponse
            {
                Id = HttpContext.GetUserSession().GpUserSession.Id      
            });
            
            return result.Accept(new PatientGuidResultVisitor());
        }
        
    }
}