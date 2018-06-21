using Microsoft.AspNetCore.Http;
using System;
using System.IO;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class Auditor : IAuditor
    {
        private readonly IAuditSink _auditSink;
        private readonly AsyncLocal<HttpContextAuditorScope> _scopeProvider;
        private readonly ILogger _logger;

        public Auditor(IAuditSink auditSink, AsyncLocal<HttpContextAuditorScope> scopeProvider, ILogger logger)
        {
            _auditSink = auditSink;
            _scopeProvider = scopeProvider;
            _logger = logger;
        }

        string NhsNumber()
        {
            string nhsNumber = null;
            nhsNumber = _scopeProvider.Value?.ToString();

            if (string.IsNullOrEmpty(nhsNumber))
            {
                throw new NoAuditKeyException(ExceptionMessages.NoNhsNumberAvailable);
            }

            return nhsNumber;
        }

        public void Audit(string operation, string details, params object[] parameters)
        {
            try
            {
                var nhsNumber = NhsNumber();

                _auditSink.WriteAudit(DateTime.Now, AuditCryptographer.Hash(nhsNumber), operation, string.Format(details, parameters));
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to write audit '{operation}'");
                throw;
            }
        }

        public IDisposable BeginScope(HttpContext httpContext)
        {
            _scopeProvider.Value = new HttpContextAuditorScope(httpContext);
            return null;
        }
    }
}