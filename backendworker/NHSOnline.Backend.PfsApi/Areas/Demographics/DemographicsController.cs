using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Demographics
{
    [ApiVersionRoute("patient")]
    public class DemographicsController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<DemographicsController> _logger;
        private readonly IDemographicsResultVisitor<IActionResult> _demographicsResultVisitor;
        private readonly IAuditor _auditor;

        public DemographicsController(
            ILogger<DemographicsController> logger,
            IGpSystemFactory gpSystemFactory,
            IDemographicsResultVisitor<IActionResult> demographicsResultVisitor,
            IAuditor auditor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _demographicsResultVisitor = demographicsResultVisitor;
            _auditor = auditor;
        }

        [HttpGet("demographics")]
        public async Task<IActionResult> Get([FromHeader(Name=PatientId)] Guid patientId, [UserSession] P9UserSession userSession)
        {
            try
            {
                _logger.LogEnter();   
                
                if (!ModelState.IsValid)
                {
                    return new BadRequestObjectResult(ModelState);
                }
                
                _logger.LogDebug($"{nameof(Get)} with patientId {patientId}");
                
                await _auditor.Audit(AuditingOperations.GetDemographicsAuditTypeRequest,
                    "Attempting to view Demographics");
                
                _logger.LogDebug($"Fetching DemographicsService for supplier: {userSession.GpUserSession.Supplier}");
                var demographicsService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetDemographicsService();

                var gpLinkedAccountUserSession = new GpLinkedAccountModel(
                    userSession.GpUserSession, patientId
                );
                
                _logger.LogDebug("Fetching Demographics");
                var result = await demographicsService.GetDemographics(gpLinkedAccountUserSession);

                await result.Accept(new DemographicsAuditingVisitor(_auditor, _logger));
                
                return result.Accept(_demographicsResultVisitor);
            }
            finally
            {
                _logger.LogExit();
            }
        }
    }
}