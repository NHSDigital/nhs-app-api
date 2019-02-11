using System;
using System.Linq;
using NHSOnline.Backend.Worker.GpSystems.Im1Connection;
using NHSOnline.Backend.Worker.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.Worker.Areas.Im1Connection
{
    public class Im1ConnectionRegisterAuditingVisitor : IIm1ConnectionRegisterResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<Im1ConnectionController> _logger;
        private readonly Supplier _supplier;
        
        private const string AuditType = Constants.AuditingTitles.Im1ConnectionRegisterResponse;

        public Im1ConnectionRegisterAuditingVisitor(IAuditor auditor, ILogger<Im1ConnectionController> logger, Supplier supplier)
        {
            _auditor = auditor;
            _logger = logger;
            _supplier = supplier;
        }

        public async Task Visit(Im1ConnectionRegisterResult.SuccessfullyRegistered result)
        {
            try
            {
                if (!string.IsNullOrEmpty(result.Response.NhsNumbers?.FirstOrDefault()?.NhsNumber))
                {
                    await _auditor.AuditWithExplicitNhsNumber(
                        result.Response.NhsNumbers.First().NhsNumber, _supplier,
                        Constants.AuditingTitles.Im1ConnectionRegisterResponse, "IM1 connection successfully registered with GP system.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionRegisterResult.SuccessfullyRegistered)}");
            }
        }

        public async Task Visit(Im1ConnectionRegisterResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType,"IM1 Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionRegisterResult.BadRequest)}");
            }
        }

        public async Task Visit(Im1ConnectionRegisterResult.InsufficientPermissions result)
        {
            try
            {
                await _auditor.Audit(AuditType,"IM1 Insufficient Permissions");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionRegisterResult.InsufficientPermissions)}");
            }
        }

        public async Task Visit(Im1ConnectionRegisterResult.NotFound result)
        {
            try
            {
                await _auditor.Audit(AuditType,"IM1 Not Found");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionRegisterResult.NotFound)}");
            }
        }

        public async Task Visit(Im1ConnectionRegisterResult.AccountAlreadyExists result)
        {
            try
            {
                await _auditor.Audit(AuditType,"IM1 Account Already Exists");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionRegisterResult.AccountAlreadyExists)}");
            }
        }

        public async Task Visit(Im1ConnectionRegisterResult.SupplierSystemUnavailable result)
        {
            try
            {
                await _auditor.Audit(AuditType,"IM1 Supplier System Unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionRegisterResult.SupplierSystemUnavailable)}");
            }
        }
    }
}
