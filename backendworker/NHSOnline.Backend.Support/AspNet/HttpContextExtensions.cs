using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support.AspNet
{
    public static class HttpContextExtensions
    {
        public static UserSession GetUserSession(this HttpContext httpContext)
        {
            return (UserSession)httpContext.Items[Constants.HttpContextItems.UserSession];
        }
        public static void SetUserSession(this HttpContext httpContext, UserSession value)
        {
            httpContext.Items[Constants.HttpContextItems.UserSession] = value;
        }
    }
}
