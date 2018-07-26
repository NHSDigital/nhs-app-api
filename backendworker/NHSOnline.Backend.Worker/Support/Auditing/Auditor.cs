using Microsoft.AspNetCore.Http;
using System;
using System.Globalization;
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

        private string NhsNumber()
        {
            var nhsNumber = _scopeProvider.Value?.UserContext()?.NhsNumber;

            if (string.IsNullOrEmpty(nhsNumber))
            {
                throw new NoAuditKeyException(ExceptionMessages.NoNhsNumberAvailable);
            }

            return nhsNumber;
        }

        private SupplierEnum Supplier()
        {
            var supplierEnum = _scopeProvider.Value?.UserContext()?.Supplier ?? SupplierEnum.Unknown;
            return supplierEnum;
        }


        public void Audit(string operation, string details, params object[] parameters)
        {
            try
            {
                var nhsNumber = NhsNumber();
                var supplier = Supplier();

                AuditWithNoTryCatch(nhsNumber, supplier, operation, details, parameters);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to write audit '{operation}'");
                throw;
            }
        }

        public void AuditWithExplicitNhsNumber(
            string nhsNumber, 
            SupplierEnum supplier, 
            string operation,
            string details, 
            params object[] parameters
        )
        {
            if (string.IsNullOrEmpty(nhsNumber))
            {
                throw new NoAuditKeyException(ExceptionMessages.NoNhsNumberAvailable);
            }
            if (supplier == SupplierEnum.Unknown)
            {
                throw new NoAuditKeyException(ExceptionMessages.SupplierNotSpecified);
            }

            try
            {
                AuditWithNoTryCatch(nhsNumber, supplier, operation, details, parameters);
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, $"Failed to write audit '{operation}'");
                throw;
            }
        }

        private void AuditWithNoTryCatch(string nhsNumber, SupplierEnum supplier, string operation, string details,
            params object[] parameters)
        {
            _auditSink.WriteAudit(DateTime.Now, AuditCryptographer.Hash(nhsNumber), supplier, operation,
                string.Format(CultureInfo.GetCultureInfo("en-GB"), details, parameters));
        }

        public IDisposable BeginScope(HttpContext httpContext)
        {
            _scopeProvider.Value = new HttpContextAuditorScope(httpContext);
            return null;
        }
    }
}
