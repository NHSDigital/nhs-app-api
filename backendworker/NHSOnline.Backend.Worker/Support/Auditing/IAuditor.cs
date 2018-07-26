using System;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public interface IAuditor
    {
        void Audit(string operation, string details, params object[] parameters);

        void AuditWithExplicitNhsNumber(string nhsnumber, SupplierEnum supplier, string operation, string details, params object[] parameters);

        IDisposable BeginScope(HttpContext httpContext);
    }
}