using Microsoft.AspNetCore.Http;
using System;

namespace NHSOnline.Backend.Worker.Support.Logging
{
    public class HttpContextLoggerScope
    {
        HttpContext _httpContext;

        public HttpContextLoggerScope(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public override string ToString()
        {
            var httpContext = (_httpContext as HttpContext);
            return String.Format("SessionId:{0}",
                httpContext.Items.Keys.Contains("UserSession") ? ((UserSession)httpContext.Items["UserSession"]).Key : "{No Session yet}");
        }
    }
}
