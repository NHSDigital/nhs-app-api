using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Worker.Conventions;
using NHSOnline.Backend.Worker.GpSystems;
using NHSOnline.Backend.Worker.Support.Auditing;
using NHSOnline.Backend.Worker.Support.Logging;

namespace NHSOnline.Backend.Worker.Areas.Session
{
    [Route("session/extend"), PfsSecurityMode]
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
        public async Task<IActionResult> Post()
        {
            try
            {
                _logger.LogEnter();

                var userSession = HttpContext.GetUserSession();

                var sessionExtendedResultVisited = await GetSessionExtendResultVisitorOutput(userSession);
                if (!sessionExtendedResultVisited.SessionWasExtended)
                {
                    _logger.LogError(
                        $"Extending the session failed with status code: '{sessionExtendedResultVisited.StatusCode}'");
                    return new StatusCodeResult(sessionExtendedResultVisited.StatusCode);
                }

                await _auditor.Audit(Constants.AuditingTitles.SessionExtendResponse, "Session successfully extended.");

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
            UserSession userSession)
        {
            _logger.LogDebug($"Fetch session extend Service for GP System: '{userSession.GpUserSession.Supplier}'.");
            var gpSystem = _gpSystemFactory.CreateGpSystem(userSession.GpUserSession.Supplier);
            var sessionService = gpSystem.GetSessionExtendService();
            var extendResult = await sessionService.Extend(userSession);
            return extendResult.Accept(new SessionExtendResultVisitor());
        }
    }
}