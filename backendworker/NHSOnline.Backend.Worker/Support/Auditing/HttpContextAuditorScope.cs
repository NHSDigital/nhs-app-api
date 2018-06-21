using Microsoft.AspNetCore.Http;
using System;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class HttpContextAuditorScope
    {
        readonly HttpContext _httpContext;

        public HttpContextAuditorScope(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public override string ToString()
        {
            if (_httpContext.Items.Keys.Contains(Constants.HttpContextItems.UserSession) == false)
            {
                return String.Empty;
            }

            return _httpContext.GetUserSession().NhsNumber;
        }
    }
}