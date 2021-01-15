using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface ISessionExpiryCookieCreator
    {
        string GetSessionExpiryCookieToken();
        void AppendSessionExpiryCookie(HttpContext context, string token);
    }
}
