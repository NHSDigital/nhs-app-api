using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support.AspNet
{
    public static class HttpContextExtensions
    {
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
