using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Linkage;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.CidApi.Areas.Linkage
{
    internal class LinkageResultAuditingVisitor<T> : ILinkageResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<T> _logger;
        private readonly Supplier _supplier;
        private readonly string _nhsNumber;
        private readonly string _auditType;

        public LinkageResultAuditingVisitor(IAuditor auditor, ILogger<T> logger, Supplier supplier, string nhsNumber,
            string auditType)
        {
            _auditor = auditor;
            _logger = logger;
            _supplier = supplier;
            _nhsNumber = nhsNumber;
            _auditType = auditType;
        }

        public async Task Visit(LinkageResult.SuccessfullyRetrieved result)
        {
            try
            {
                await _auditor.AuditRegistrationEvent(_nhsNumber, _supplier, _auditType,
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
                await _auditor.AuditRegistrationEvent(_nhsNumber, _supplier, _auditType, "Linkage details successfully retrieved - already existed.");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SuccessfullyRetrievedAlreadyExists)}");
            }
        }

        public async Task Visit(LinkageResult.SuccessfullyCreated result)
        {
            try
            {
                await _auditor.AuditRegistrationEvent(_nhsNumber, _supplier, _auditType, "Linkage key successfully created.");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SuccessfullyCreated)}");
            }
        }

        public async Task Visit(LinkageResult.SupplierSystemUnavailable result)
        {
            try
            {
                await _auditor.AuditRegistrationEvent(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful due to supplier being unavailable.");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {_auditType} {nameof(LinkageResult.SupplierSystemUnavailable)}");
            }
        }

        public async Task Visit(LinkageResult.InternalServerError result)
        {
            try
            {
                await _auditor.AuditRegistrationEvent(_nhsNumber, _supplier, _auditType, "Linkage details request unsuccessful due to internal server error.");
            }
            catch (Exception e)
            {
                _logger.LogError(e,
                    $"Exception thrown auditing {_auditType} {nameof(LinkageResult.InternalServerError)}");
            }
        }


        public async Task Visit(LinkageResult.ErrorCase result)
        {
            try
            {
                await _auditor.AuditRegistrationEvent(_nhsNumber, _supplier, _auditType, "Error when linking user");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameof(LinkageResult.ErrorCase)}");
            }
        }

        public async Task Visit(LinkageResult.NotFound result)
        {
            await AuditError("Not Found", nameof(LinkageResult.NotFound));
        }

        public async Task Visit(LinkageResult.UnmappedErrorWithStatusCode result)
        {
            await AuditError("Unmapped Error", nameof(LinkageResult.UnmappedErrorWithStatusCode));
        }

        private async Task AuditError(string errorType, string nameOfResultType)
        {
            try
            {
                await _auditor.AuditRegistrationEvent(_nhsNumber, _supplier, _auditType, $"{errorType} returned when linking user - unknown error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {_auditType} {nameOfResultType}");
            }
        }
    }
}