using System;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    public class CreatePrescriptionResultAuditingVisitor :  IPrescriptionResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly string _courseIds;
        
        private const string AuditType = Constants.AuditingTitles.RepeatPrescriptionsOrderRepeatMedicationsResponse;

        public CreatePrescriptionResultAuditingVisitor(IAuditor auditor, ILogger<PrescriptionsController> logger, string courseIds)
        {
            _auditor = auditor;
            _logger = logger;
            _courseIds = courseIds;
        }

        public async Task Visit(PrescriptionResult.SuccessfulGet result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Prescriptions successfully retrieved with course ids: {0}", _courseIds);
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
                await _auditor.Audit(AuditType, "Repeat prescription request successfully created with course ids: {0}", _courseIds);
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
                await _auditor.Audit(AuditType, "Error creating prescription request: Supplier Unavailable with course ids: {0}", _courseIds);
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
                await _auditor.Audit(AuditType, "Error creating prescription request: Supplier Not Enabled with course ids: {0}", _courseIds);
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
                await _auditor.Audit(AuditType, "Error creating prescription request: Internal Server Error with course ids: {0}", _courseIds);
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
                await _auditor.Audit(AuditType, "Error creating prescription request: Bad Request with course ids: {0}", _courseIds);
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
                await _auditor.Audit(AuditType, "Error creating prescription request: Cannot Reorder Prescription with course ids: {0}", _courseIds);
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
                await _auditor.Audit(AuditType, "Error ordering prescription: Medication already ordered within last 30 days with course ids: {0}", _courseIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(PrescriptionResult.MedicationAlreadyOrderedWithinLast30Days)}");
            }
        }
    }
}
