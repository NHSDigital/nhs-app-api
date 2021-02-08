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
    public class Im1ConnectionVerifyAuditingVisitor : IIm1ConnectionVerifyResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<Im1ConnectionController> _logger;
        private readonly Supplier _supplier;

        private const string AuditType = AuditingOperations.Im1ConnectionVerifyResponse;

        public Im1ConnectionVerifyAuditingVisitor(IAuditor auditor, ILogger<Im1ConnectionController> logger,
            Supplier supplier)
        {
            _auditor = auditor;
            _logger = logger;
            _supplier = supplier;
        }

        public async Task Visit(Im1ConnectionVerifyResult.Success result)
        {
            try
            {
                if (!string.IsNullOrEmpty(result.Response.NhsNumbers?.FirstOrDefault()?.NhsNumber))
                {
                    await _auditor.PostOperationAuditRegistrationEvent(
                        result.Response.NhsNumbers.First().NhsNumber, _supplier,
                        AuditingOperations.Im1ConnectionVerifyResponse,
                        "IM1 connection successfully verified with GP system.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionVerifyResult.Success)}");
            }
        }

        public Task Visit(Im1ConnectionVerifyResult.BadGateway result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(Im1ConnectionVerifyResult.BadRequest result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(Im1ConnectionVerifyResult.UnmappedErrorWithStatusCode result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(Im1ConnectionVerifyResult.ErrorCase result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(Im1ConnectionVerifyResult.Forbidden result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(Im1ConnectionVerifyResult.InternalServerError result)
        {
            return Task.CompletedTask;
        }

        public Task Visit(Im1ConnectionVerifyResult.NotFound result)
        {
            return Task.CompletedTask;
        }

    }
}
