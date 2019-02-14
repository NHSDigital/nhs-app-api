using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Support.Auditing
{
    public interface IAuditor
    {
        Task Audit(string operation, string details, params object[] parameters);
        
        Task PostAudit(string operation, string details, params object[] parameters);

        Task AuditWithExplicitNhsNumber(string nhsNumber, Supplier supplier, string operation, string details, params object[] parameters);

        IDisposable BeginScope(HttpContext httpContext);
    }
}