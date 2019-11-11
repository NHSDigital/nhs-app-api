using System;
using System.IO;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.AspNet;
using Constants = NHSOnline.Backend.Support.Constants;

namespace NHSOnline.Backend.PfsApi.Filters
{
    [AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
    public sealed class ProxyingNotAllowedAttribute : Attribute, IAuthorizationFilter
    {
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            var logger = context.HttpContext.RequestServices.GetService<ILogger<ProxyingNotAllowedAttribute>>();
            var userSession = context.HttpContext.GetUserSession();

            if (logger == null) throw new InvalidDataException(nameof(logger));
            if (userSession == null) throw new InvalidDataException(nameof(userSession));

            var loggedInPatientIdFromSession = userSession.GpUserSession.Id;
            var patientIdInRequestHeader = context.HttpContext.Request.Headers[Constants.HttpHeaders.PatientId];

            if (!string.Equals(loggedInPatientIdFromSession.ToString(), patientIdInRequestHeader.ToString(), StringComparison.Ordinal))
            {
                logger.LogWarning($"action requires header {nameof(Constants.HttpHeaders.PatientId)} to match id of session user");
                context.Result = new StatusCodeResult(StatusCodes.Status403Forbidden);
                return;
            }
        }
    }
}
