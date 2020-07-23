using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface ICreateSessionResultVisitor<out T>
    {
        T Visit(CreateSessionResult.Success success, HttpContext httpContext);
        T Visit(CreateSessionResult.ErrorResult errorResultResult);
    }
}