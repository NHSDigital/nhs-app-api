using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    public class GetPrescriptionsResultAuditingVisitor : IGetPrescriptionsResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly FilteringCounts _prescriptionCount;
        
        private const string AuditType = AuditingOperations.RepeatPrescriptionsViewHistoryResponse;

        public GetPrescriptionsResultAuditingVisitor(IAuditor auditor, ILogger<PrescriptionsController> logger, FilteringCounts prescriptionCount)
        {
            _auditor = auditor;
            _logger = logger;
            _prescriptionCount = prescriptionCount;
        }
        
        public async Task Visit(GetPrescriptionsResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, 
                    "Prescriptions successfully retrieved. " +
                    $"Total prescriptions before filtering: {_prescriptionCount.ReceivedCount}, " +
                    $"Total prescriptions returned after filtering: {_prescriptionCount.ReturnedCount}");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPrescriptionsResult.Success)}");
            }
        }

        public async Task Visit(GetPrescriptionsResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving prescriptions: Supplier Unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPrescriptionsResult.BadGateway)}");
            }
        }

        public async Task Visit(GetPrescriptionsResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving prescriptions: Insufficient permissions");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPrescriptionsResult.Forbidden)}");
            }
        }

        public async Task Visit(GetPrescriptionsResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving prescriptions: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPrescriptionsResult.InternalServerError)}");
            }
        }

        public async Task Visit(GetPrescriptionsResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error retrieving prescriptions: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPrescriptionsResult.BadRequest)}");
            }
        }
    }
}
