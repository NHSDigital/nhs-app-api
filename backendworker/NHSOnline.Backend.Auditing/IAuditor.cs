using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auditing
{
    public interface IAuditor
    {
        Task PreOperationAudit(string operation, string details, params object[] parameters);

        Task PostOperationAudit(string operation, string details, params object[] parameters);

        Task PreOperationAuditRegistrationEvent(string nhsNumber, Supplier supplier, string operation, string details,
            params object[] parameters);

        Task PostOperationAuditRegistrationEvent(string nhsNumber, Supplier supplier, string operation, string details,
            params object[] parameters);

         Task PostOperationAuditSessionEvent(string accessToken, string nhsNumber, Supplier supplier, string operation, string details,
             params object[] parameters);

        IDisposable BeginScope(HttpContext httpContext);

        IAuditBuilder Audit();
    }
}
