using System;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auth.CitizenId.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Auditing
{
    public class Auditor : IAuditor
    {
        private readonly IAuditSink _auditSink;
        private readonly AsyncLocal<HttpContextAuditorScope> _scopeProvider;
        private readonly ILogger<Auditor> _logger;
        private readonly IConfiguration _configuration;

        public Auditor(IAuditSink auditSink, AsyncLocal<HttpContextAuditorScope> scopeProvider, ILogger<Auditor> logger, IConfiguration configuration)
        {
            _auditSink = auditSink ?? throw new ArgumentNullException(nameof(auditSink));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Audit(string operation, string details, params object[] parameters)
        {
            var nhsNumber = _scopeProvider.Value?.UserContext()?.NhsNumber;
            var supplier = _scopeProvider.Value?.UserContext()?.Supplier ?? Supplier.Unknown;
            var accessToken = _scopeProvider.Value?.UserContext()?.AccessToken;
            var nhsLoginSubject = DeriveNhsLoginSubject(accessToken);

            var isProxying = _scopeProvider.Value?.UserContext().IsProxying ?? false;

            if (isProxying)
            {
                nhsNumber = _scopeProvider.Value?.UserContext().LinkedAccountNhsNumber;
            }
            
            await AuditInternal(nhsLoginSubject, nhsNumber, isProxying, supplier, operation, details, parameters);
        }

        public async Task AuditRegistrationEvent(
            string nhsNumber, 
            Supplier supplier, 
            string operation,
            string details, 
            params object[] parameters
        )
        {
            const string nhsLoginSubject = "";
            await AuditInternal(nhsLoginSubject, nhsNumber, false, supplier, operation, details, parameters);
        }

        public async Task AuditSessionEvent(
            string accessToken,
            string nhsNumber,
            Supplier supplier,
            string operation,
            string details,
            params object[] parameters
        )
        {
            var nhsLoginSubject = DeriveNhsLoginSubject(accessToken);
            await AuditInternal(nhsLoginSubject, nhsNumber, false, supplier, operation, details, parameters);
        }

        public Task AuditSecureTokenEvent(AccessToken accessToken, Supplier supplier, string operation, string details,
            params object[] parameters)
        {
            return AuditInternal(accessToken.Subject, accessToken.NhsNumber, false, supplier, operation, details, parameters);
        }

        public IDisposable BeginScope(HttpContext httpContext)
        {
            _scopeProvider.Value = new HttpContextAuditorScope(httpContext, _configuration);
            return null;
        }

        private async Task AuditInternal(
            string nhsLoginSubject, 
            string nhsNumber,
            bool isProxying,
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
            
            var auditRecord = BuildAuditRecord(nhsLoginSubject, nhsNumber, isProxying, supplier, operation, details, parameters);

            await AuditInternal(auditRecord);
        }

        private async Task AuditInternal(AuditRecord auditRecord)
        {
            try
            {
                await _auditSink.WriteAudit(auditRecord);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Failed to write audit '{auditRecord.Operation}'");
                throw;
            }
        }

        private AuditRecord BuildAuditRecord(string nhsLoginSubject, string nhsNumber, bool isProxying, Supplier supplier, string operation,
            string details, params object[] parameters)
        {
            var formattedDetails = string.Format(CultureInfo.GetCultureInfo("en-GB"), details, parameters);           
            var versionTag = _scopeProvider.Value?.VersionTag();

            var auditRecord = new AuditRecord(
                DateTime.UtcNow,
                nhsLoginSubject,
                nhsNumber,
                isProxying,
                supplier,
                operation,
                formattedDetails,
                versionTag
            );

            return auditRecord;
        }

        private string DeriveNhsLoginSubject(string accessToken)
        {
            try
            {
                var token = AccessToken.Parse(_logger, accessToken);
                return token.Subject;
            }
            catch (Exception e)
            {
                throw new NoAuditKeyException(ExceptionMessages.AccessTokenInvalid, e);
            }
        }
    }
}
