using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    [ApiVersionRoute("session/extend")]
    public class SessionExtendController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<SessionExtendController> _logger;
        private readonly IAuditor _auditor;

        public SessionExtendController(IGpSystemFactory gpSystemFactory,
            ILogger<SessionExtendController> logger,
            IAuditor auditor)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
            _auditor = auditor;
        }

        [HttpPost]
        public async Task<IActionResult> Post([FromHeader(Name=PatientId)] Guid patientId)
        {
            try
            {
                _logger.LogEnter();

                var userSession = HttpContext.GetUserSession();

                var sessionExtendedResultVisited = await GetSessionExtendResultVisitorOutput(userSession, patientId);

                if (!sessionExtendedResultVisited.SessionWasExtended)
                {
                    _logger.LogError(
                        $"Extending the session failed with status code: '{sessionExtendedResultVisited.StatusCode}'");
                    return new StatusCodeResult(sessionExtendedResultVisited.StatusCode);
                }

                await _auditor.Audit(AuditingOperations.SessionExtendResponse, "Session successfully extended.");

                _logger.LogDebug(
                    $"Finished session extend post with status code {sessionExtendedResultVisited.StatusCode}");

                return new StatusCodeResult(sessionExtendedResultVisited.StatusCode);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private async Task<SessionExtendResultVisitorOutput> GetSessionExtendResultVisitorOutput(
            P9UserSession userSession, Guid patientId)
        {
            _logger.LogDebug($"Fetch session extend Service for GP System: '{userSession.GpUserSession.Supplier}'.");
            var sessionService = _gpSystemFactory
                .CreateGpSystem(userSession.GpUserSession.Supplier)
                .GetSessionExtendService();
            
            var gpLinkedAccountModel = new GpLinkedAccountModel(
                userSession.GpUserSession, patientId);

            var extendResult = await sessionService.Extend(gpLinkedAccountModel);

            return extendResult.Accept(new SessionExtendResultVisitor());
        }
    }
}