using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.LinkedAccounts;
using NHSOnline.Backend.GpSystems.LinkedAccounts.Models;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;
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
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ISessionCacheService _sessionCacheService;


        public ServiceJourneyRulesController(
            ILogger<ServiceJourneyRulesController> logger,
            IAuditor auditor,
            IServiceJourneyRulesService serviceJourneyRulesService,
            SessionConfigurationSettings sessionSettings,
            IGpSystemFactory gpSystemFactory,
            ISessionCacheService sessionCacheService
        )
        {
            _logger = logger;
            _auditor = auditor;
            _serviceJourneyRulesService = serviceJourneyRulesService;
            _sessionSettings = sessionSettings;
            _gpSystemFactory = gpSystemFactory;
            _sessionCacheService = sessionCacheService;
        }

        [HttpGet]
        [ApiVersionRoute("patient/journey-configuration")]
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
        [ApiVersionRoute("patient/configuration")]
        public async Task<IActionResult> GetLinkedAccountPatientConfig()
        {
            await _auditor.Audit(AuditingOperations.GetPatientConfigRequest,"Attempting to get config for patient");

            var userSession = HttpContext.GetUserSession();
            var linkedAccountsBreakdown = new LinkedAccountsBreakdownSummary();

            var gpSystem = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier);

            if (gpSystem.SupportsLinkedAccounts
                && userSession.GpUserSession.HasLinkedAccounts)
            {
                var linkedAccountsService = gpSystem.GetLinkedAccountsService();

                var linkedAccountsResult = await linkedAccountsService.GetLinkedAccounts(userSession.GpUserSession);

                if (linkedAccountsResult is LinkedAccountsResult.Success success)
                {
                    linkedAccountsBreakdown = success.LinkedAccountsBreakdown;
                    if (success.HasAnyProxyInfoBeenUpdatedInSession)
                    {
                        _logger.LogInformation("Updating session as proxy info has been updated");
                        await _sessionCacheService.UpdateUserSession(userSession);
                    }
                }
            }

            LinkedAccountsConfigResult result = new LinkedAccountsConfigResult.Success(
                userSession.GpUserSession.Id,
                _sessionSettings,
                linkedAccountsBreakdown);

            await result.Accept(new LinkedAccountConfigResultAuditingVisitor(_auditor, _logger));
            return result.Accept(new LinkedAccountsConfigResultVisitor());
        }

        [HttpGet]
        [ApiVersionRoute("patient/linked-account-journey-configuration")]
        public async Task<IActionResult> GetLinkedAccountConfiguration([FromHeader(Name = PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();

                await _auditor.Audit(AuditingOperations.GetServiceJourneyRulesAuditForLinkedAccountRequest,
                    "Attempting to get service journey rules for linked account");

                var gpUserSession = HttpContext.GetUserSession().GpUserSession;

                var linkedAccountsService =
                    _gpSystemFactory.CreateGpSystem(gpUserSession.Supplier).GetLinkedAccountsService();

                string odsCode = linkedAccountsService.GetOdsCodeForLinkedAccount(gpUserSession, patientId);

                _logger.LogInformation($"Fetching Service Journey Rules for linked account");

                var result = await _serviceJourneyRulesService.GetServiceJourneyRulesForLinkedAccount(odsCode);

                return result.Accept(new ServiceJourneyRulesGetResultVisitor());
            }
            finally
            {
                _logger.LogExit();
            }
        }

    }
}