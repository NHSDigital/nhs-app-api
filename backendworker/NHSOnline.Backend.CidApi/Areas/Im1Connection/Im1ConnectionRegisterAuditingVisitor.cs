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
        private readonly IIm1ConnectionErrorCodes _errorCodes;
        private readonly Supplier _supplier;
        
        private const string AuditType = Constants.AuditingTitles.Im1ConnectionRegisterResponse;

        public Im1ConnectionRegisterAuditingVisitor(IAuditor auditor, ILogger<Im1ConnectionController> logger, IIm1ConnectionErrorCodes errorCodes,  Supplier supplier)
        {
            _auditor = auditor;
            _logger = logger;
            _errorCodes = errorCodes;
            _supplier = supplier;
        }

        public async Task Visit(Im1ConnectionRegisterResult.Success result)
        {
            try
            {
                if (!string.IsNullOrEmpty(result.Response.NhsNumbers?.FirstOrDefault()?.NhsNumber))
                {
                    await _auditor.AuditRegistrationEvent(
                        result.Response.NhsNumbers.First().NhsNumber, _supplier,
                        Constants.AuditingTitles.Im1ConnectionRegisterResponse, "IM1 connection successfully registered with GP system.");
                }
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(Im1ConnectionRegisterResult.Success)}");
            }
        }

        public Task Visit(Im1ConnectionRegisterResult.BadRequest result)
        {
            LogErrorCode(result.ErrorCode, nameof(Im1ConnectionRegisterResult.BadRequest));
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionRegisterResult.NotFound result)
        {
            LogErrorCode(result.ErrorCode, nameof(Im1ConnectionRegisterResult.NotFound));
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionRegisterResult.Conflict result)
        {
            LogErrorCode(result.ErrorCode, nameof(Im1ConnectionRegisterResult.Conflict));
            return Task.FromResult<object>(null);
        }

        public Task Visit(Im1ConnectionRegisterResult.BadGateway result)
        {
            return Task.FromResult<object>(null);
        }
        
        public Task Visit(Im1ConnectionRegisterResult.UnknownError result)
        {
            LogErrorCode(result.ErrorCode, nameof(Im1ConnectionRegisterResult.UnknownError));
            return Task.FromResult<object>(null);
        }
        public Task Visit(Im1ConnectionRegisterResult.ErrorCase result)
        {
            LogErrorCode(result.ErrorCode, nameof(Im1ConnectionRegisterResult.ErrorCase));
            return Task.FromResult<object>(null);
        }

        private void LogErrorCode(Im1ConnectionErrorCodes.Code resultErrorCode, string nameOfResult)
        {
            _logger.LogInformation($"Linkage result: '{nameOfResult}'. " +
                                   $"Error code: {resultErrorCode} : {(int)resultErrorCode} : {_errorCodes.GetErrorResponse(resultErrorCode).ErrorMessage}");
        }
    }
}
