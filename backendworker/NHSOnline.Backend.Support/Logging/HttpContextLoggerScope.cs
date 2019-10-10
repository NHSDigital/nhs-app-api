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
            var items = _httpContext?.Items;
            object value = null;
            items?.TryGetValue(Constants.HttpContextItems.UserSession, out value);

            var userSession = value as UserSession ?? new UserSession { Key = "{No Session yet}" };

            return $"SessionId:{userSession.Key}";
        }
    }
}
