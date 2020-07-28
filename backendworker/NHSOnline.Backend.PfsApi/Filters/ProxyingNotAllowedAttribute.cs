using System;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support.Session;
using Constants = NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ProxyingNotAllowedAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var httpContext = context.HttpContext;
            var logger = httpContext.RequestServices.GetRequiredService<ILogger<ProxyingNotAllowedAttribute>>();
            var p9UserSessionOpt = httpContext.RequestServices
                .GetRequiredService<IUserSessionService>()
                .GetUserSession<P9UserSession>();

            p9UserSessionOpt.IfSome(p9UserSession =>
            {
                if (IsProxying(p9UserSession, httpContext))
                {
                    logger.LogWarning(
                            $"action requires header {nameof(Constants.HttpHeaders.PatientId)} to match id of session user");
                        context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                }
            });
        }

        private static bool IsProxying(P9UserSession userSession, HttpContext httpContext)
        {
            var gpSystem = httpContext.RequestServices
                .GetRequiredService<IGpSystemFactory>()
                .CreateGpSystem(userSession.GpUserSession.Supplier);

            if (!gpSystem.SupportsLinkedAccounts)
            {
                return false;
            }

            var loggedInPatientIdFromSession = userSession.GpUserSession.Id;
            var patientIdInRequestHeader = httpContext.Request.Headers[Constants.HttpHeaders.PatientId];

            return !string.Equals(
                loggedInPatientIdFromSession.ToString(),
                patientIdInRequestHeader.ToString(),
                StringComparison.Ordinal);
        }
    }
}
