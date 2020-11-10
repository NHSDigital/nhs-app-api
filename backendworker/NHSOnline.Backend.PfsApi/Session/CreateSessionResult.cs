using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    public abstract class CreateSessionResult
    {
        private CreateSessionResult() { }

        internal abstract T Accept<T>(ICreateSessionResultVisitor<T> visitor, HttpContext httpContext, string referrer);

        public sealed class Success: CreateSessionResult
        {
            internal Success(ServiceJourneyRulesResponse serviceJourneyRules, UserSession userSession)
            {
                ServiceJourneyRules = serviceJourneyRules;
                UserSession = userSession;
            }

            internal ServiceJourneyRulesResponse ServiceJourneyRules { get; }
            internal UserSession UserSession { get; }
            internal override T Accept<T>(ICreateSessionResultVisitor<T> visitor, HttpContext httpContext, string referrer) => visitor.Visit(this, httpContext, referrer);
        }

        public sealed class ErrorResult: CreateSessionResult
        {
            internal ErrorResult(ErrorTypes errorTypes) => ErrorTypes = errorTypes;

            internal ErrorTypes ErrorTypes { get; }
            internal override T Accept<T>(ICreateSessionResultVisitor<T> visitor, HttpContext httpContext, string referrer) => visitor.Visit(this);
        }
    }
}