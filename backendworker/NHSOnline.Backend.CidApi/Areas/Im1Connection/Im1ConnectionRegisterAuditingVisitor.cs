using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Im1Connection;
using NHSOnline.Backend.Support;

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

        private const string AuditType = AuditingOperations.Im1ConnectionRegisterResponse;

        public Im1ConnectionRegisterAuditingVisitor(IAuditor auditor, ILogger<Im1ConnectionController> logger,  Supplier supplier)
        {
            _auditor = auditor;
            _logger = logger;
            _supplier = supplier;
        }

        public async Task Visit(Im1ConnectionRegisterResult.Success result)
        {
            try
            {
                if (!string.IsNullOrEmpty(result.Response.NhsNumbers?.FirstOrDefault()?.NhsNumber))
                {
                    await _auditor.PostOperationAuditRegistrationEvent(
                        result.Response.NhsNumbers.First().NhsNumber, _supplier,
                        AuditingOperations.Im1ConnectionRegisterResponse, "IM1 connection successfully registered with GP system.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionRegisterResult.Success)}");
            }
        }

        public Task Visit(Im1ConnectionRegisterResult.BadGateway result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(Im1ConnectionRegisterResult.UnmappedErrorWithStatusCode result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(Im1ConnectionRegisterResult.ErrorCase result)
        {
            return Task.CompletedTask;
        }
    }
}
