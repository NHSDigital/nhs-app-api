using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auditing
{
    public interface IAuditor
    {
        Task Audit(string operation, string details, params object[] parameters);

        Task AuditRegistrationEvent(string nhsNumber, Supplier supplier, string operation, string details,
            params object[] parameters);

        Task AuditSessionEvent(string accessToken, string nhsNumber, Supplier supplier, string operation, string details,
            params object[] parameters);

        Task AuditSecureTokenEvent(AccessToken accessToken, Supplier supplier, string operation,
            string details, params object[] parameters);

        IDisposable BeginScope(HttpContext httpContext);
    }
}