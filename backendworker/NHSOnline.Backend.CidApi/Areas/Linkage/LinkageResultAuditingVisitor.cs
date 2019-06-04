using System;
using System.Threading.Tasks;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems.Im1Connection;

namespace NHSOnline.Backend.CidApi.Areas.Linkage
{
    internal class LinkageResultAuditingVisitor<T> : ILinkageResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<T> _logger;
        private readonly IIm1ConnectionErrorCodes _errorCodes;
        private readonly Supplier _supplier;
        private readonly string _nhsNumber;
        private readonly string _auditType;
        
        public LinkageResultAuditingVisitor(IAuditor auditor, ILogger<T> logger, IIm1ConnectionErrorCodes errorCodes, Supplier supplier, string nhsNumber, string auditType)
        {
            _auditor = auditor;
            _logger = logger;
            _errorCodes = errorCodes;
            _supplier = supplier;
            _nhsNumber = nhsNumber;
            _auditType = auditType;
        }
        
        public async Task Visit(LinkageResult.SuccessfullyRetrieved result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType,
                    "Linkage details successfully retrieved.");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SuccessfullyRetrieved)}");
            }
        }

        public async Task Visit(LinkageResult.SuccessfullyRetrievedAlreadyExists result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details successfully retrieved - already existed.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SuccessfullyRetrievedAlreadyExists)}");
            }
        }

        public async Task Visit(LinkageResult.SuccessfullyCreated result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage key successfully created.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SuccessfullyCreated)}");
            }
        }

        public async Task Visit(LinkageResult.SupplierSystemUnavailable result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful due to supplier being unavailable.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SupplierSystemUnavailable)}");
            }
        }

        public async Task Visit(LinkageResult.InternalServerError result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful due to internal server error.");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.InternalServerError)}");
            }
        }


        public async Task Visit(LinkageResult.ErrorCase result)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, "Error when linking user");
                LogErrorCode(result.ErrorCode, nameof(LinkageResult.ErrorCase));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.ErrorCase)}");
            }
        }

        public async Task Visit(LinkageResult.NotFound result)
        {
            await AuditUnknownError("Not Found", nameof(LinkageResult.NotFound), result.ErrorCode);
        }

        public async Task Visit(LinkageResult.BadRequest result)
        {
            await AuditUnknownError("Bad Request", nameof(LinkageResult.BadRequest), result.ErrorCode);
        }

        public async Task Visit(LinkageResult.Conflict result)
        {
            await AuditUnknownError("Conflict", nameof(LinkageResult.Conflict), result.ErrorCode);
        }

        public async Task Visit(LinkageResult.Forbidden result)
        {
            await AuditUnknownError("Forbidden", nameof(LinkageResult.Forbidden), result.ErrorCode);
        }
        public async Task Visit(LinkageResult.UnknownError result)
        {
            await AuditUnknownError("Unknown Error", nameof(LinkageResult.UnknownError), result.ErrorCode);
        }

        private async Task AuditUnknownError(string errorType, string nameOfResultType, Im1ConnectionErrorCodes.Code errorCode)
        {
            try
            {
                await _auditor.AuditWithExplicitNhsNumber(_nhsNumber, _supplier, _auditType, $"{errorType} returned when linking user - unknown error");
                LogErrorCode(errorCode, nameOfResultType);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameOfResultType}");
            }
        }

        private void LogErrorCode(Im1ConnectionErrorCodes.Code resultErrorCode, string nameOfResult)
        {
            _logger.LogInformation($"Linkage result: '{nameOfResult}'. " +
                                   $"Error code: {resultErrorCode} : {(int)resultErrorCode} : {_errorCodes.GetErrorResponse(resultErrorCode).ErrorMessage}");
        }
    }
}