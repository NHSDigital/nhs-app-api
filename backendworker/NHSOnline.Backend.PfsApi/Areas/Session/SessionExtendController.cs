using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Session;
using static NHSOnline.Backend.Support.Constants.HttpHeaders;

namespace NHSOnline.Backend.PfsApi.Areas.Session
{
    [ApiVersionRoute("session/extend")]
    public class SessionExtendController : Controller
    {
        private readonly IGpSystemFactory _gpSystemFactory;
        private readonly ILogger<SessionExtendController> _logger;

        public SessionExtendController(IGpSystemFactory gpSystemFactory,
            ILogger<SessionExtendController> logger)
        {
            _gpSystemFactory = gpSystemFactory;
            _logger = logger;
        }

        [HttpPost]
        public async Task<IActionResult> Post(
            [FromHeader(Name=PatientId)] Guid patientId,
            [UserSession] P5UserSession userSession)
        {
            try
            {
                _logger.LogEnter();

                var sessionExtendVisitor = new UserSessionExtendVisitor(_logger, _gpSystemFactory, patientId);
                var sessionExtendedResultVisited = await userSession.Accept(sessionExtendVisitor);

                if (!sessionExtendedResultVisited.SessionWasExtended)
                {
                    _logger.LogError(
                        $"Extending the session failed with status code: '{sessionExtendedResultVisited.StatusCode}'");
                    return new StatusCodeResult(sessionExtendedResultVisited.StatusCode);
                }

                _logger.LogDebug(
                    $"Finished session extend post with status code {sessionExtendedResultVisited.StatusCode}");

                return new StatusCodeResult(sessionExtendedResultVisited.StatusCode);
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private sealed class UserSessionExtendVisitor : IUserSessionVisitor<Task<SessionExtendResultVisitorOutput>>
        {
            private readonly ILogger _logger;
            private readonly IGpSystemFactory _gpSystemFactory;
            private readonly Guid _patientId;

            public UserSessionExtendVisitor(ILogger logger, IGpSystemFactory gpSystemFactory, Guid patientId)
            {
                _logger = logger;
                _gpSystemFactory = gpSystemFactory;
                _patientId = patientId;
            }


            public Task<SessionExtendResultVisitorOutput> Visit(P5UserSession userSession)
                => Task.FromResult(new SessionExtendResultVisitorOutput { SessionWasExtended = true, StatusCode = StatusCodes.Status200OK });

            public async Task<SessionExtendResultVisitorOutput> Visit(P9UserSession userSession)
            {
                _logger.LogDebug($"Fetch session extend Service for GP System: '{userSession.GpUserSession.Supplier}'.");
                var sessionService = _gpSystemFactory
                    .CreateGpSystem(userSession.GpUserSession.Supplier)
                    .GetSessionExtendService();

                var gpLinkedAccountModel = new GpLinkedAccountModel(
                    userSession.GpUserSession, _patientId);

                var extendResult = await sessionService.Extend(gpLinkedAccountModel);

                return extendResult.Accept(new SessionExtendResultVisitor());
            }
        }
    }
}