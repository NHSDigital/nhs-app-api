using Microsoft.AspNetCore.Http;
using System;

namespace NHSOnline.Backend.Worker.Support.Logging
{
    public class HttpContextLoggerScope
    {
        readonly HttpContext _httpContext;

        public HttpContextLoggerScope(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public override string ToString()
        {
            return String.Format("SessionId:{0}",
                _httpContext.Items.Keys.Contains("UserSession") ? ((UserSession)_httpContext.Items["UserSession"]).Key : "{No Session yet}");
        }
    }
}
