using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Authorization;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.RequestFormatters;
using NHSOnline.Backend.PfsApi.Filters;
using NHSOnline.Backend.Support.AspNet.Filters;

namespace NHSOnline.Backend.PfsApi
{
    internal static class PfsMvcOptions
    {
        internal static void Configure(MvcOptions options)
        {
            options.Filters.Add(typeof(HttpContextAuditActionFilterAttribute), 1);
            options.Filters.Add(new AuthorizeFilter(
                new AuthorizationPolicyBuilder().RequireAuthenticatedUser().Build())
            );

            /* NB - order of adding these filters is important. LIFO stack is used, and the optional
             *      'Order' parameter appears to be ignored.
             *      Therefore please ensure UnhandledExceptionFilterAttribute is added first, so that
             *      it is invoked as a last resort. */
            options.Filters.Add(typeof(UnhandledExceptionFilterAttribute));
            options.Filters.Add(typeof(TimeoutExceptionFilterAttribute));
            options.Filters.Add(typeof(UnparsableExceptionFilterAttribute));
            options.Filters.Add(typeof(UnauthorisedGpSystemHttpRequestExceptionFilterAttribute));
            options.Filters.Add(typeof(InvalidPatientIdExceptionFilterAttribute));
            options.Filters.Add<UserSessionFilter>();

            options.InputFormatters.Insert(0, new FhirParametersInputFormatter());
        }
    }
}