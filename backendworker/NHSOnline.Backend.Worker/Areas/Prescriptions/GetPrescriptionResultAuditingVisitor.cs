using System;
using System.Linq;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.Worker.Areas.Prescriptions
{
    public class GetPrescriptionResultAuditingVisitor :  IPrescriptionResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PrescriptionsController> _logger;
        
        private const string AuditType = Constants.AuditingTitles.RepeatPrescriptionsViewHistoryResponse;

        public GetPrescriptionResultAuditingVisitor(IAuditor auditor, ILogger<PrescriptionsController> logger)
        {
            _auditor = auditor;
            _logger = logger;
        }
        
        public async Task Visit(PrescriptionResult.SuccessfulGet result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, $"Prescriptions successfully retrieved - { result.Response?.Prescriptions?.Select(x => x.Courses?.Count()).Sum() } courses");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.SuccessfulGet)}");
            }
        }

        public async Task Visit(PrescriptionResult.SuccessfulPost result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Repeat prescription request successfully created");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.SuccessfulPost)}");
            }
        }

        public async Task Visit(PrescriptionResult.SupplierSystemUnavailable result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Supplier Unavailable");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.SupplierSystemUnavailable)}");
            }
        }

        public async Task Visit(PrescriptionResult.SupplierNotEnabled result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Supplier Not Enabled");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.SupplierNotEnabled)}");
            }
        }

        public async Task Visit(PrescriptionResult.InternalServerError result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Internal Server Error");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.InternalServerError)}");
            }
        }

        public async Task Visit(PrescriptionResult.BadRequest result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Bad Request");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.BadRequest)}");
            }
        }

        public async Task Visit(PrescriptionResult.CannotReorderPrescription result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Cannot Reorder Prescription");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.CannotReorderPrescription)}");
            }
        }

        public async Task Visit(PrescriptionResult.MedicationAlreadyOrderedWithinLast30Days result)
        {
            try
            {
                await _auditor.PostAudit(AuditType, "Error retrieving prescriptions: Medication already ordered within last 30 days");
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.MedicationAlreadyOrderedWithinLast30Days)}");
            }
        }
    }
}
