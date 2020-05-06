using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.Session;
using NHSOnline.Backend.Support.Http;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class UnauthorisedGpSystemHttpRequestExceptionFilterAttribute : ExceptionFilterAttribute
    {
        private readonly IUserSessionService _userSessionService;
        private readonly IUserSessionManager _userSessionManager;
        private readonly IAuditor _auditor;
        private readonly ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> _logger;

        public UnauthorisedGpSystemHttpRequestExceptionFilterAttribute(
            IUserSessionService userSessionService,
            IUserSessionManager userSessionManager,
            IAuditor auditor,
            ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> logger)
        {
            _userSessionService = userSessionService;
            _userSessionManager = userSessionManager;
            _auditor = auditor;
            _logger = logger;
        }

        public override async Task OnExceptionAsync(ExceptionContext context)
        {
            if (context.ExceptionHandled)
            {
                return;
            }
            
            if (context.Exception is UnauthorisedGpSystemHttpRequestException)
            {
                _logger.LogWarning($"{nameof(UnauthorisedGpSystemHttpRequestException)} was caught - returning {nameof(StatusCodes.Status401Unauthorized)}");
                _logger.LogDebug($"{context.Exception}");

                var userSession = _userSessionService.GetUserSession<UserSession>();

                await userSession
                    .IfSome<Task>(async session => await DeleteUserSession(context.HttpContext, session))
                    .IfNone(Task.CompletedTask);

                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                context.ExceptionHandled = true;
            }
        }

        private async Task<DeleteUserSessionResult> DeleteUserSession(HttpContext httpContext, UserSession session)
        {
            // Exception filters execute outside of action filters so we need to setup the auditor scope manually
            using var _ = _auditor.BeginScope(httpContext);

            return await _userSessionManager.Delete(httpContext, session);
        }
    }
}
