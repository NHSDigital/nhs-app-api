using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class HttpContextAuditorScope
    {
        readonly HttpContext _httpContext;

        public HttpContextAuditorScope(HttpContext httpContext)
        {
            _httpContext = httpContext;
        }

        public AuditUserContext UserContext()
        {
            if (_httpContext.Items.Keys.Contains(Constants.HttpContextItems.UserSession) == false)
            {
                return new AuditUserContext(null, Supplier.Unknown);
            }

            var userSession = _httpContext.GetUserSession();

            var auditUserContext = new AuditUserContext(userSession.NhsNumber, userSession.Supplier);

            return auditUserContext;
        }
    }
}
