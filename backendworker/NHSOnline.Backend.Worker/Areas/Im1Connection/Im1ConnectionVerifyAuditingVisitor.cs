using System;
using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    /// <summary>
    /// The Im1 Registration endpoint deliberately doesn’t audit anything in the case of failures as it doesn’t have an NHS number against which to log the audit entry.
    /// </summary>
    public class Im1ConnectionVerifyAuditingVisitor : IIm1ConnectionVerifyResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<Im1ConnectionController> _logger;
        private readonly Supplier _supplier;

        private const string AuditType = Constants.AuditingTitles.Im1ConnectionVerifyResponse;

        public Im1ConnectionVerifyAuditingVisitor(IAuditor auditor, ILogger<Im1ConnectionController> logger,
            Supplier supplier)
        {
            _auditor = auditor;
            _logger = logger;
            _supplier = supplier;
        }

        public async Task Visit(Im1ConnectionVerifyResult.SuccessfullyVerified result)
        {
            try
            {
                if (!string.IsNullOrEmpty(result.Response.NhsNumbers?.FirstOrDefault()?.NhsNumber))
                {
                    await _auditor.AuditWithExplicitNhsNumber(
                        result.Response.NhsNumbers.First().NhsNumber, _supplier,
                        Constants.AuditingTitles.Im1ConnectionVerifyResponse,
                        "IM1 connection successfully verified with GP system.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.SuccessfullyVerified)}");
            }
        }

        public Task Visit(Im1ConnectionVerifyResult.InsufficientPermissions result)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionVerifyResult.SupplierSystemUnavailable result)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionVerifyResult.InvalidUserCredentials invalidUserCredentials)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionVerifyResult.InvalidRequest invalidRequest)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionVerifyResult.UnknownError unknownError)
        {
            return Task.FromResult<object>(null);
        }
    }
}