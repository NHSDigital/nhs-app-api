using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
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
        private readonly ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> _logger;

        public UnauthorisedGpSystemHttpRequestExceptionFilterAttribute(
            IUserSessionService userSessionService,
            IUserSessionManager userSessionManager,
            ILogger<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute> logger)
        {
            _userSessionService = userSessionService;
            _userSessionManager = userSessionManager;
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
                    .IfSome<Task>(async session => await _userSessionManager.Delete(context.HttpContext, session))
                    .IfNone(Task.CompletedTask);

                context.Result = new StatusCodeResult(StatusCodes.Status401Unauthorized);
                context.ExceptionHandled = true;
            }
        }
    }
}
