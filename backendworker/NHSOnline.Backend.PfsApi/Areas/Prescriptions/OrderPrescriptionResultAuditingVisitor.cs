using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Prescriptions;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    public class OrderPrescriptionResultAuditingVisitor :  IOrderPrescriptionResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly string _courseIds;
        
        private const string AuditType = AuditingOperations.RepeatPrescriptionsOrderRepeatMedicationsResponse;

        public OrderPrescriptionResultAuditingVisitor(IAuditor auditor, ILogger<PrescriptionsController> logger, string courseIds)
        {
            _auditor = auditor;
            _logger = logger;
            _courseIds = courseIds;
        }

        public async Task Visit(OrderPrescriptionResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Repeat prescription request successfully created with course ids: {0}", _courseIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrderPrescriptionResult.Success)}");
            }
        }

        public async Task Visit(OrderPrescriptionResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error creating prescription request: Supplier Unavailable with course ids: {0}", _courseIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrderPrescriptionResult.BadGateway)}");
            }
        }

        public async Task Visit(OrderPrescriptionResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error creating prescription request: Insufficient permissions with course ids: {0}", _courseIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrderPrescriptionResult.Forbidden)}");
            }
        }

        public async Task Visit(OrderPrescriptionResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error creating prescription request: Internal Server Error with course ids: {0}", _courseIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrderPrescriptionResult.InternalServerError)}");
            }

        }

        public async Task Visit(OrderPrescriptionResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error creating prescription request: Bad Request with course ids: {0}", _courseIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrderPrescriptionResult.BadRequest)}");
            }
        }

        public async Task Visit(OrderPrescriptionResult.CannotReorderPrescription result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error creating prescription request: Cannot Reorder Prescription with course ids: {0}", _courseIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrderPrescriptionResult.CannotReorderPrescription)}");
            }
        }

        public async Task Visit(OrderPrescriptionResult.MedicationAlreadyOrderedWithinLast30Days result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Error ordering prescription: Medication already ordered within last 30 days with course ids: {0}", _courseIds);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrderPrescriptionResult.MedicationAlreadyOrderedWithinLast30Days)}");
            }
        }
    }
}
