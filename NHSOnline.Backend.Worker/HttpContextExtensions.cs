using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Worker
{
    public static class HttpContextExtensions
    {
        public static UserSession GetUserSession(this HttpContext httpContext)
        {
            return (UserSession)httpContext.Items[Constants.HttpContextItems.UserSession];
        }
    }
}
