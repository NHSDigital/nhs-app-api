using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionLoggerScope
    {
        private const string NoSessionYet = "{No Session yet}";

        private readonly IUserSessionService _userSessionService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public SessionLoggerScope(
            IUserSessionService userSessionService,
            IHttpContextAccessor httpContextAccessor)
        {
            _userSessionService = userSessionService;
            _httpContextAccessor = httpContextAccessor;
        }

        public override string ToString()
        {
            var userSessionKey =_userSessionService
                .GetUserSession<P5UserSession>()
                .Select(userSession => userSession.Key)
                .ValueOr(() => NoSessionYet) ?? NoSessionYet;

            var linkedAccountAuditInfo = _httpContextAccessor.HttpContext?.GetLinkedAccountAuditInfo();

            if (linkedAccountAuditInfo?.IsProxyMode == true)
            {
                return $"SessionId:{userSessionKey} | In proxy mode";
            }

            return $"SessionId:{userSessionKey}";
        }
    }
}