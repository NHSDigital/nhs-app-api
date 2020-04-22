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
    internal sealed class Auditor : IAuditor
    {
        private readonly IAuditSink _auditSink;
        private readonly AsyncLocal<HttpContextAuditorScope> _scopeProvider;
        private readonly ILogger _logger;
        private readonly IConfiguration _configuration;

        public Auditor(IAuditSink auditSink, AsyncLocal<HttpContextAuditorScope> scopeProvider, ILogger logger, IConfiguration configuration)
        {
            _auditSink = auditSink ?? throw new ArgumentNullException(nameof(auditSink));
            _scopeProvider = scopeProvider ?? throw new ArgumentNullException(nameof(scopeProvider));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));
        }

        public async Task Audit(string operation, string details, params object[] parameters)
        {
            var auditUserContext = _scopeProvider.Value?.UserContext()
                                   ?? throw new NoAuditKeyException("Cannot audit outside of HttpContextAuditorScope");

            var nhsNumber = auditUserContext.NhsNumber;
            var supplier = auditUserContext.Supplier;
            var accessToken = auditUserContext.AccessToken;
            var nhsLoginSubject = DeriveNhsLoginSubject(accessToken);

            var isProxying = auditUserContext.IsProxying;

            if (isProxying)
            {
                nhsNumber = auditUserContext.LinkedAccountNhsNumber;
            }
            
            await AuditInternal(nhsLoginSubject, nhsNumber, isProxying, supplier, operation, details, parameters);
        }

        public async Task AuditRegistrationEvent(
            string nhsNumber, 
            Supplier supplier, 
            string operation,
            string details, 
            params object[] parameters)
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
            params object[] parameters)
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

        public IAuditBuilder Audit()
        {
            var scope = _scopeProvider.Value ??
                        throw new InvalidOperationException("Cannot audit outside of HttpContextAuditorScope");
            var state = new AuditBuilderState(
                _logger,
                _auditSink,
                () => scope.UserContext(),
                scope.VersionTag());
            return new AuditBuilder(state);
        }

        private async Task AuditInternal(
            string nhsLoginSubject, 
            string nhsNumber,
            bool isProxying,
            Supplier supplier,
            string operation,
            string details,
            params object[] parameters)
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

        private AuditRecord BuildAuditRecord(
            string nhsLoginSubject,
            string nhsNumber,
            bool isProxying,
            Supplier supplier,
            string operation,
            string details,
            params object[] parameters)
        {
            if (parameters.Length > 0)
            {
                details = string.Format(CultureInfo.GetCultureInfo("en-GB"), details, parameters);
            }

            var versionTag = _scopeProvider.Value?.VersionTag();

            var auditRecord = new AuditRecord(
                DateTime.UtcNow,
                nhsLoginSubject,
                nhsNumber,
                isProxying,
                supplier,
                operation,
                details,
                versionTag);

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

        private sealed class AuditBuilderState
        {
            private readonly Func<AuditUserContext> _auditUserContext;
            private readonly VersionTag _versionTag;

            public AuditBuilderState(ILogger logger, IAuditSink auditSink, Func<AuditUserContext> auditUserContext, VersionTag versionTag)
            {
                Logger = logger;
                AuditSink = auditSink;
                _auditUserContext = auditUserContext;
                _versionTag = versionTag;
            }

            public ILogger Logger { get; }
            public IAuditSink AuditSink { get; }

            public string NhsLoginSubject { get; set; } = string.Empty;
            public string NhsNumber { get; set; } = string.Empty;
            public bool IsProxying { get; set; }
            public Supplier Supplier { get; set; }
            public string Operation { get; set; }
            public string Details { get; set; }

            internal AuditRecord BuildAuditRecord(string operationSuffix)
                => new AuditRecord(
                    DateTime.UtcNow,
                    NhsLoginSubject,
                    NhsNumber,
                    IsProxying,
                    Supplier,
                    $"{Operation}_{operationSuffix}",
                    Details,
                    _versionTag);

            internal void SetContextFromScope()
            {
                var auditUserContext = _auditUserContext();
                NhsNumber = auditUserContext.NhsNumber;
                Supplier = auditUserContext.Supplier;
                DeriveNhsLoginSubject(auditUserContext.AccessToken);

                IsProxying = auditUserContext.IsProxying;

                if (IsProxying)
                {
                    NhsNumber = auditUserContext.LinkedAccountNhsNumber;
                }
            }

            internal void DeriveNhsLoginSubject(string accessToken)
            {
                try
                {
                    var token = AccessToken.Parse(Logger, accessToken);
                    NhsLoginSubject = token.Subject;
                }
                catch (Exception e)
                {
                    throw new NoAuditKeyException(ExceptionMessages.AccessTokenInvalid, e);
                }
            }
        }

        private sealed class AuditBuilder : IAuditBuilder
        {
            public AuditBuilder(AuditBuilderState state) => State = state;

            private AuditBuilderState State { get; }

            public IAuditBuilderNhsNumber AccessToken(string accessToken)
            {
                State.DeriveNhsLoginSubject(accessToken);
                return new AuditBuilderContext(State);
            }

            public IAuditBuilderNhsNumber AccessToken(AccessToken accessToken)
            {
                State.NhsLoginSubject = accessToken.Subject;
                return new AuditBuilderContext(State);
            }

            public IAuditBuilderSupplier NhsNumber(string nhsNumber)
            {
                State.NhsNumber = nhsNumber;
                return new AuditBuilderContext(State);
            }

            public IAuditBuilderDetails Operation(string operation)
            {
                State.SetContextFromScope();
                return new AuditBuilderOperation(State).Operation(operation);
            }
        }

        private sealed class AuditBuilderContext : IAuditBuilderNhsNumber, IAuditBuilderSupplier
        {
            public AuditBuilderContext(AuditBuilderState state) => State = state;

            private AuditBuilderState State { get; }

            public IAuditBuilderSupplier NhsNumber(string nhsNumber)
            {
                State.NhsNumber = nhsNumber;
                return this;
            }

            public IAuditBuilderOperation Supplier(Supplier supplier)
            {
                State.Supplier = supplier;
                return new AuditBuilderOperation(State);
            }
        }

        private sealed class AuditBuilderOperation : IAuditBuilderOperation, IAuditBuilderDetails, IAuditBuilderExecute
        {
            public AuditBuilderOperation(AuditBuilderState state)
            {
                State = state;
            }

            private AuditBuilderState State { get; }

            public IAuditBuilderDetails Operation(string operation)
            {
                State.Operation = operation;
                return this;
            }

            public IAuditBuilderExecute Details(string details)
            {
                State.Details = details;
                return this;
            }

            public IAuditBuilderExecute Details(string details, params object[] parameters)
                => Details(string.Format(CultureInfo.GetCultureInfo("en-GB"), details, parameters));

            public async Task<TAuditedResult> Execute<TAuditedResult>(Func<Task<TAuditedResult>> auditedAction) where TAuditedResult : IAuditedResult
            {
                await AuditRequest();

                TAuditedResult result;
                try
                {
                    result = await auditedAction();
                }
                catch (Exception e)
                {
                    await TryAuditResponse(e.Message);
                    throw;
                }

                await TryAuditResponse(result.Details);
                return result;
            }

            private async Task AuditRequest()
            {
                try
                {
                    await Audit("Request");
                }
                catch (Exception e)
                {
                    State.Logger.LogError(e, $"Failed to audit {State.Operation} request: {State.Details}");
                    throw;
                }
            }

            private async Task TryAuditResponse(string details)
            {
                try
                {
                    await Audit("Response");
                }
                catch (Exception e)
                {
                    State.Logger.LogError(e, $"Failed to audit {State.Operation} response: {details}");
                }
            }

            private async Task Audit(string operationSuffix)
            {
                var record = State.BuildAuditRecord(operationSuffix);
                await State.AuditSink.WriteAudit(record);
            }
        }
    }
}
