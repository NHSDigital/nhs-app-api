using Microsoft.AspNetCore.Http;
using System;

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
                return new AuditUserContext(null, SupplierEnum.Unknown);
            }

            var userSession = _httpContext.GetUserSession();

            var auditUserContext = new AuditUserContext(userSession.NhsNumber, userSession.Supplier);

            return auditUserContext;
        }
    }
}
