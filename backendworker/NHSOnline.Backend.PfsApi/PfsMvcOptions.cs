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
        /**
         *  NB - order of adding these filters is important. LIFO stack is used, and the optional
         *      'Order' parameter appears to be ignored.
         *      Therefore please ensure UnhandledExceptionFilterAttribute is added first, so that
         *      it is invoked as a last resort.
         */
        internal static void Configure(MvcOptions options)
        {
            var filters = options.Filters;
            var authPolicy = new AuthorizationPolicyBuilder()
                .RequireAuthenticatedUser()
                .Build();
            var authFilter = new AuthorizeFilter(authPolicy);

            filters.Add<HttpContextAuditActionFilterAttribute>(1);
            filters.Add(authFilter);
            filters.Add<UnhandledExceptionFilterAttribute>();
            filters.Add<TimeoutExceptionFilterAttribute>();
            filters.Add<UnparsableExceptionFilterAttribute>();
            filters.Add<UnauthorisedGpSystemHttpRequestExceptionFilterAttribute>();
            filters.Add<InvalidPatientIdExceptionFilterAttribute>();
            filters.Add<UserSessionFilter>();
            // the GP session filter requires auditing, so it must invoked after the audit filter
            filters.Add<GpSessionFilter>(2);

            options.InputFormatters.Insert(0, new FhirParametersInputFormatter());
        }
    }
}