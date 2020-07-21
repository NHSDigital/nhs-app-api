using System.Net;
using NHSOnline.App.Api.Client.Errors;

namespace NHSOnline.App.Api.Client.Session
{
    internal abstract class ApiCreateSessionResult
    {
        private ApiCreateSessionResult() { }

        internal abstract T Accept<T>(IApiCreateSessionResultVisitor<T> visitor);

        internal sealed class Success: ApiCreateSessionResult
        {
            internal Success(UserSessionResponse userSessionResponse, CookieContainer cookies)
            {
                UserSessionResponse = userSessionResponse;
                Cookies = cookies;
            }

            internal UserSessionResponse UserSessionResponse { get; }
            internal CookieContainer Cookies { get; }

            internal override T Accept<T>(IApiCreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Failure : ApiCreateSessionResult
        {
            internal override T Accept<T>(IApiCreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class BadRequest : ApiCreateSessionResult
        {
            public BadRequest(PfsErrorResponse pfsErrorResponse) => PfsErrorResponse = pfsErrorResponse;

            internal PfsErrorResponse PfsErrorResponse { get; }

            internal override T Accept<T>(IApiCreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class Forbidden : ApiCreateSessionResult
        {
            public Forbidden(PfsErrorResponse pfsErrorResponse) => PfsErrorResponse = pfsErrorResponse;

            internal PfsErrorResponse PfsErrorResponse { get; }

            internal override T Accept<T>(IApiCreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class OdsCodeNotSupportedOrNoNhsNumber : ApiCreateSessionResult
        {
            public OdsCodeNotSupportedOrNoNhsNumber(PfsErrorResponse pfsErrorResponse) => PfsErrorResponse = pfsErrorResponse;

            internal PfsErrorResponse PfsErrorResponse { get; }

            internal override T Accept<T>(IApiCreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        internal sealed class FailedAgeRequirement : ApiCreateSessionResult
        {
            public FailedAgeRequirement(PfsErrorResponse pfsErrorResponse) => PfsErrorResponse = pfsErrorResponse;

            internal PfsErrorResponse PfsErrorResponse { get; }

            internal override T Accept<T>(IApiCreateSessionResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}