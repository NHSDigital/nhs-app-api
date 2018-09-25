using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
using System.Threading;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace NHSOnline.Backend.Worker.Support.Auditing
{
    public class Auditor : IAuditor
    {
        private readonly IAuditSink _auditSink;
        private readonly AsyncLocal<HttpContextAuditorScope> _scopeProvider;
        private readonly ILogger<Auditor> _logger;

        public Auditor(IAuditSink auditSink, AsyncLocal<HttpContextAuditorScope> scopeProvider, ILogger<Auditor> logger)
        {
            _auditSink = auditSink ?? throw new ArgumentNullException(nameof(auditSink));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        private string NhsNumber()
        {
            var nhsNumber = _scopeProvider.Value?.UserContext()?.NhsNumber;

            if (string.IsNullOrEmpty(nhsNumber))
            {
                throw new NoAuditKeyException(ExceptionMessages.NoNhsNumberAvailable);
            }

            return nhsNumber;
        }

        private Supplier Supplier()
        {
            var supplier = _scopeProvider.Value?.UserContext()?.Supplier ?? Worker.Supplier.Unknown;
            return supplier;
        }

        public async Task Audit(string operation, string details, params object[] parameters)
        {
            try
            {
                var nhsNumber = NhsNumber();
                var supplier = Supplier();

                await AuditWithNoTryCatch(nhsNumber, supplier, operation, details, parameters);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to write audit '{operation}'");
                throw;
            }
        }

        public async Task AuditWithExplicitNhsNumber(
            string nhsNumber, 
            Supplier supplier, 
            string operation,
            string details, 
            params object[] parameters
        )
        {
            if (string.IsNullOrEmpty(nhsNumber))
            {
                throw new NoAuditKeyException(ExceptionMessages.NoNhsNumberAvailable);
            }
            if (supplier == Worker.Supplier.Unknown)
            {
                throw new NoAuditKeyException(ExceptionMessages.SupplierNotSpecified);
            }

            try
            {
                await AuditWithNoTryCatch(nhsNumber, supplier, operation, details, parameters);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to write audit '{operation}'");
                throw;
            }
        }

        private async Task AuditWithNoTryCatch(string nhsNumber, Supplier supplier, string operation, string details,
            params object[] parameters)
        {
            await _auditSink.WriteAudit(DateTime.UtcNow, nhsNumber, supplier, operation,
                string.Format(CultureInfo.GetCultureInfo("en-GB"), details, parameters));
        }

        public IDisposable BeginScope(HttpContext httpContext)
        {
            _scopeProvider.Value = new HttpContextAuditorScope(httpContext);
            return null;
        }
    }
}
