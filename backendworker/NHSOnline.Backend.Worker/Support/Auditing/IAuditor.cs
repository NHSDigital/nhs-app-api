using System;
using Microsoft.AspNetCore.Http;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public interface IAuditor
    {
        void Audit(string details, string message, params object[] parameters);
        IDisposable BeginScope(HttpContext httpContext);
    }
}