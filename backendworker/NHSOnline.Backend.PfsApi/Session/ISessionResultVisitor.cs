using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface ISessionResultVisitor<out T>
    {
        T Visit(CreateSessionResult.Success success, HttpContext httpContext, string sessionCookieExpiryToken, string referrer, string integrationReferrer);

        T Visit(CreateSessionResult.Success success, HttpContext httpContext, string referrer);

        T Visit(CreateSessionResult.ErrorResult errorResultResult);

        T Visit(CreateSessionResult.GpSessionExists gpSessionExists);
    }
}
