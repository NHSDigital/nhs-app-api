using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.PfsApi.Session
{
    public interface ISessionExpiryCookieCreator
    {
        string CreateSessionExpiryToken();
        void AppendSessionExpiryCookie(HttpContext context, string token);
        string DecodeSessionExpiryToken(string encryptedCookie);
    }
}
