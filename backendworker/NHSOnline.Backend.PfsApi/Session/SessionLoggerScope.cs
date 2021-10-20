using System;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Session
{
    internal sealed class SessionLoggerScope
    {
        private const string NoSessionYet = "{No Session yet}";
        private const string NoSessionCookie = "{EmptySessionCookie}";

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
            var userSessionKey = GetSessionIdFromSessionObject();

            if (userSessionKey.Equals(NoSessionYet, StringComparison.OrdinalIgnoreCase))
            {
                userSessionKey = GetSessionFromSessionCookie();
            }

            var linkedAccountAuditInfo = _httpContextAccessor.HttpContext?.GetLinkedAccountAuditInfo();

            if (linkedAccountAuditInfo?.IsProxyMode == true)
            {
                return $"SessionId:{userSessionKey} | In proxy mode";
            }

            return $"SessionId:{userSessionKey}";
        }

        private string GetSessionIdFromSessionObject()
        {
            return _userSessionService
                .GetUserSession<P5UserSession>()
                .Select(userSession => userSession.Key)
                .ValueOr(() => NoSessionYet) ?? NoSessionYet;
        }

        private string GetSessionFromSessionCookie()
        {
            if (!(_httpContextAccessor.HttpContext?.Request.Cookies.Count > 0))
            {
                return NoSessionYet;
            }

            var sessionCookie = _httpContextAccessor.HttpContext?.Request.Cookies[Constants.CookieNames.SessionId];

            if (sessionCookie is null)
            {
                return NoSessionCookie;
            }

            return Guid.TryParse(sessionCookie, out var guid) ? $"{guid}" : $"{sessionCookie}";
        }
    }
}