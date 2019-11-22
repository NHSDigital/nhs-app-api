using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support.AspNet;

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
            var items = _httpContext?.Items;
            object value = null;
            items?.TryGetValue(Constants.HttpContextItems.UserSession, out value);

            var userSession = value as UserSession ?? new UserSession { Key = "{No Session yet}" };

            if (userSession.GpUserSession != null) //valid session has been retrieved
            {
                var linkedAccountAuditInfo = _httpContext.GetLinkedAccountAuditInfo();

                if (linkedAccountAuditInfo?.IsProxyMode == true)
                {
                    return $"SessionId:{userSession.Key} | In proxy mode";                   
                }
            }
            return $"SessionId:{userSession.Key}";
        }
    }
}
