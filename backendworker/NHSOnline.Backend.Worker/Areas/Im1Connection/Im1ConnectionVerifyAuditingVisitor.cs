using System;
using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
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

        public async Task Visit(Im1ConnectionVerifyResult.InsufficientPermissions result)
        {
            try
            {
                await _auditor.Audit(AuditType,"Insufficient permissions to verify connection.");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.InsufficientPermissions)}");
            }
        }

        public async Task Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            try
            {
                await _auditor.Audit(AuditType,"IM1 Connection not found");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.NotFound)}");
            }
        }

        public async Task Visit(Im1ConnectionVerifyResult.SupplierSystemUnavailable result)
        {
            try
            {
                await _auditor.Audit(AuditType,"IM1 Supplier System Unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.SupplierSystemUnavailable)}");
            }
        }

        public async Task Visit(Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader errorProcessingSecurityHeader)
        {
            try
            {
                await _auditor.Audit(AuditType,"Error processing IM1 security header");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.ErrorProcessingSecurityHeader)}");
            }
        }

        public async Task Visit(Im1ConnectionVerifyResult.InvalidUserCredentials invalidUserCredentials)
        {
            try
            {
                await _auditor.Audit(AuditType,"Invalid IM1 User Credentials");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.InvalidUserCredentials)}");
            }
        }

        public async Task Visit(Im1ConnectionVerifyResult.InvalidRequest invalidRequest)
        {
            try
            {
                await _auditor.Audit(AuditType,"Invalid IM1 Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.InvalidRequest)}");
            }
        }

        public async Task Visit(Im1ConnectionVerifyResult.UnknownError unknownError)
        {
            try
            {
                await _auditor.Audit(AuditType,"Unknown IM1 error");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.UnknownError)}");
            }
        }
    }
}