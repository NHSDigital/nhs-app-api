using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Session;
using NHSOnline.Backend.GpSystems.SessionManager;
using NHSOnline.Backend.PfsApi.GpSession;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

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
            ISessionCacheService sessionCacheService)
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
        public async Task<IActionResult> Get([UserSession] P5UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var odsCode = userSession.OdsCode;

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
        public async Task<IActionResult> GetLinkedAccountPatientConfig(
            [GpSession(IgnoreP5Users=true)] GpUserSession gpUserSession,
            [UserSession] P5UserSession userSession)
        {
            var visitor = new LinkedAccountPatientConfigVisitor(_logger, _auditor, _sessionSettings, _gpSystemFactory, _sessionCacheService, gpUserSession);
            var result = await userSession.Accept(visitor);

            return result.Accept(new LinkedAccountsConfigResultVisitor());
        }

        [HttpGet]
        [ApiVersionRoute("patient/linked-account-journey-configuration")]
        public async Task<IActionResult> GetLinkedAccountConfiguration(
            [FromHeader(Name = PatientId)] Guid patientId,
            [GpSession] GpUserSession gpUserSession)
        {
            try
            {
                _logger.LogEnter();

                var linkedAccountsService =
                    _gpSystemFactory.CreateGpSystem(gpUserSession.Supplier).GetLinkedAccountsService();

                var odsCode = linkedAccountsService.GetOdsCodeForLinkedAccount(gpUserSession, patientId);

                _logger.LogInformation("Fetching Service Journey Rules for linked account");

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