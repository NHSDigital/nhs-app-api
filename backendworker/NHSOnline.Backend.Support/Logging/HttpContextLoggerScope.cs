using System.Globalization;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support.Logging
{
    public class HttpContextLoggerScope
    {
        private readonly HttpContext _httpContext;

        public HttpContextLoggerScope(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "SessionId:{0}",
                _httpContext != null && _httpContext.Items.Keys.Contains("UserSession")
                    ? ((UserSession) _httpContext.Items["UserSession"]).Key
                    : "{No Session yet}");
        }
    }
}
