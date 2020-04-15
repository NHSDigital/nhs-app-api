using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.Auditing
{
    public class HttpContextAuditorScope
    {
        private readonly HttpContext _httpContext;
        private readonly string _appApiVersion;

        public HttpContextAuditorScope(
            HttpContext httpContext, 
            IConfiguration configuration)
        {
            _httpContext = httpContext;
            _appApiVersion = configuration[Constants.EnvironmentalVariables.VersionTag];
        }

        public VersionTag VersionTag()
        {
            string webAppVersion = _httpContext.Request.Headers[Constants.HttpHeaders.WebAppVersion];
            string nativeAppVersion = _httpContext.Request.Headers[Constants.HttpHeaders.NativeAppVersion];

            if (_appApiVersion == null || webAppVersion == null)
            {
                return null;
            }
            
            return new VersionTag(_appApiVersion, webAppVersion, nativeAppVersion);
        }

        public AuditUserContext UserContext()
        {
            var userSession = _httpContext.RequestServices
                .GetService<IUserSessionService>()
                ?.GetUserSession<UserSession>() ?? Option.None<UserSession>();
            if (userSession.IsEmpty)
            {
                return new AuditUserContext(null,null, Supplier.Unknown, false, null);
            }
            var isProxying = false;
            string linkedAccountNhsNumber = null;

            if (_httpContext.Items.Keys.Contains(Constants.HttpContextItems.LinkedAccountAuditInfo))
            {
                var linkedAccountAuditInfo = _httpContext.GetLinkedAccountAuditInfo();
                isProxying = linkedAccountAuditInfo.IsProxyMode;
                linkedAccountNhsNumber = linkedAccountAuditInfo.ProxyNhsNumber;
            }

            return userSession.ValueOrFailure().Accept(new AuditUserContextVisitor(isProxying, linkedAccountNhsNumber));
        }

        private sealed class AuditUserContextVisitor : IUserSessionVisitor<AuditUserContext>
        {
            private readonly bool _isProxying;
            private readonly string _linkedAccountNhsNumber;

            public AuditUserContextVisitor(bool isProxying, string linkedAccountNhsNumber)
            {
                _isProxying = isProxying;
                _linkedAccountNhsNumber = linkedAccountNhsNumber;
            }

            public AuditUserContext Visit(P5UserSession userSession)
                => new AuditUserContext(
                    userSession.CitizenIdUserSession.AccessToken,
                    string.Empty,
                    Supplier.Unknown,
                    _isProxying,
                    _linkedAccountNhsNumber);

            public AuditUserContext Visit(P9UserSession userSession)
                => new AuditUserContext(
                    userSession.CitizenIdUserSession.AccessToken,
                    userSession.GpUserSession.NhsNumber,
                    userSession.GpUserSession.Supplier,
                    _isProxying,
                    _linkedAccountNhsNumber);
        }
    }
}
