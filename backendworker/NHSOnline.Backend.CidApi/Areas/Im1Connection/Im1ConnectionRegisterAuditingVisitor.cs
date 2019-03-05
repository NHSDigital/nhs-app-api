using System;
using System.Linq;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.CidApi.Areas.Im1Connection
{   
    /// <summary>
    /// The Im1 Registration endpoint deliberately doesn’t audit anything in the case of failures as it doesn’t have an NHS number against which to log the audit entry.
    /// </summary>
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

        public Task Visit(Im1ConnectionRegisterResult.BadRequest result)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionRegisterResult.InsufficientPermissions result)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionRegisterResult.NotFound result)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionRegisterResult.AccountAlreadyExists result)
        {
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionRegisterResult.SupplierSystemUnavailable result)
        {
            return Task.FromResult<object>(null);
        }
    }
}
