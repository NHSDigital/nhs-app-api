using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support.AspNet
{
    public static class HttpContextExtensions
    {
        public static P9UserSession GetUserSession(this HttpContext httpContext)
        {
            return (P9UserSession)httpContext.Items[Constants.HttpContextItems.UserSession];
        }
        
        public static void SetUserSession(this HttpContext httpContext, P9UserSession value)
        {
            httpContext.Items[Constants.HttpContextItems.UserSession] = value;
        }
        
        public static LinkedAccountAuditInfo GetLinkedAccountAuditInfo(this HttpContext httpContext)
        {
            return (LinkedAccountAuditInfo)httpContext.Items[Constants.HttpContextItems.LinkedAccountAuditInfo];
        }
        
        public static void SetLinkedAccountAuditInfo(this HttpContext httpContext, LinkedAccountAuditInfo value)
        {
            httpContext.Items[Constants.HttpContextItems.LinkedAccountAuditInfo] = value;
        }
    }
}
