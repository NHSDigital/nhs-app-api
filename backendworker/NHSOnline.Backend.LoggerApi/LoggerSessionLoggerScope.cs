using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support;
using Org.BouncyCastle.Asn1.Ocsp;

namespace NHSOnline.Backend.LoggerApi
{
    public class LoggerSessionLoggerScope
    {
        private const string GUID_REG_EX = "[0-9a-f]{8}-[0-9a-f]{4}-[0-5][0-9a-f]{3}-[089ab][0-9a-f]{3}-[0-9a-f]{12}";

        private readonly IHttpContextAccessor _httpContextAccessor;

        public LoggerSessionLoggerScope(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }
        public override string ToString()
        {
            if (!(_httpContextAccessor.HttpContext?.Request.Cookies.Count > 0))
            {
                return "SessionID:EmptyCookies";
            }

            var sessionCookie = _httpContextAccessor.HttpContext?.Request.Cookies[Constants.CookieNames.SessionId];

            if (sessionCookie is null)
            {
                return "SessionID:EmptySessionCookie";
            }

            var guid = Regex.Match(sessionCookie,
                    @GUID_REG_EX);

            return guid.Success ? $"SessionId:{guid.Value}" : $"SessionId:{sessionCookie}";
        }
    }
}