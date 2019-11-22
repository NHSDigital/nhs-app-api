using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.AspNet;

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
            if (_httpContext.Items.Keys.Contains(Constants.HttpContextItems.UserSession) == false)
            {
                return new AuditUserContext(null,null, Supplier.Unknown, false, null);
            }
            var userSession = _httpContext.GetUserSession();
            var isProxying = false;
            string linkedAccountNhsNumber = null;

            if (_httpContext.Items.Keys.Contains(Constants.HttpContextItems.LinkedAccountAuditInfo))
            {
                var linkedAccountAuditInfo = _httpContext.GetLinkedAccountAuditInfo();
                isProxying = linkedAccountAuditInfo.IsProxyMode;
                linkedAccountNhsNumber = linkedAccountAuditInfo.ProxyNhsNumber;
            }

            return new AuditUserContext(
                    userSession.CitizenIdUserSession.AccessToken,
                    userSession.GpUserSession.NhsNumber,
                    userSession.GpUserSession.Supplier, 
                    isProxying, 
                    linkedAccountNhsNumber);
        }
    }
}
