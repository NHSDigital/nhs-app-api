using System;
using System.Linq;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    public class GetPrescriptionsResultAuditingVisitor : IGetPrescriptionsResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PrescriptionsController> _logger;
        
        private const string AuditType = Constants.AuditingTitles.RepeatPrescriptionsViewHistoryResponse;

        public GetPrescriptionsResultAuditingVisitor(IAuditor auditor, ILogger<PrescriptionsController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(GetPrescriptionsResult.Success result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, $"Prescriptions successfully retrieved - { result.Response?.Prescriptions?.Select(x => x.Courses?.Count()).Sum() } courses");
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
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Supplier Unavailable");
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
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Insufficient permissions");
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
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Internal Server Error");
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
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPrescriptionsResult.BadRequest)}");
            }
        }

        public async Task Visit(GetPrescriptionsResult.CannotReorderPrescription result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Cannot Reorder Prescription");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPrescriptionsResult.CannotReorderPrescription)}");
            }
        }

        public async Task Visit(GetPrescriptionsResult.MedicationAlreadyOrderedWithinLast30Days result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Medication already ordered within last 30 days");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(GetPrescriptionsResult.MedicationAlreadyOrderedWithinLast30Days)}");
            }
        }
    }
}
