using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.Support.Logging
{
    internal sealed class HttpContextLoggerScope
    {
        private const string NoSessionYet = "{No Session yet}";
        private readonly HttpContext _httpContext;

        internal HttpContextLoggerScope(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public override string ToString()
        {
            var userSessionKey = _httpContext?.RequestServices
                .GetService<IUserSessionService>()
                ?.GetUserSession<P5UserSession>()
                .Select(userSession => userSession.Key)
                .ValueOr(() => NoSessionYet) ?? NoSessionYet;

            var linkedAccountAuditInfo = _httpContext?.GetLinkedAccountAuditInfo();

            if (linkedAccountAuditInfo?.IsProxyMode == true)
            {
                return $"SessionId:{userSessionKey} | In proxy mode";
            }

            return $"SessionId:{userSessionKey}";
        }
    }
}
