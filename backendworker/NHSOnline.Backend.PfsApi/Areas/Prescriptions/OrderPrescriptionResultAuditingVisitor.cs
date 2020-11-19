using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Prescriptions;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Prescriptions
{
    public class OrderPrescriptionResultAuditingVisitor :  IOrderPrescriptionResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<PrescriptionsController> _logger;
        private readonly string _courseIds;
        private readonly IMetricLogger _metricLogger;
        private readonly P9UserSession _userSession;

        private const string AuditType = AuditingOperations.RepeatPrescriptionsOrderRepeatMedicationsResponse;

        public OrderPrescriptionResultAuditingVisitor(IAuditor auditor,
            ILogger<PrescriptionsController> logger,
            string courseIds,
            IMetricLogger metricLogger,
            P9UserSession userSession)
        {
            _auditor = auditor;
            _logger = logger;
            _courseIds = courseIds;
            _metricLogger = metricLogger;
            _userSession = userSession;
        }

        public async Task Visit(OrderPrescriptionResult.Success result)
        {
            try
            {
                await _metricLogger.RepeatPrescriptionOrder(new RepeatPrescriptionData(_userSession.Key));
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

        public async Task Visit(OrderPrescriptionResult.PartialSuccess result)
        {
            try
            {
                await _auditor.Audit(
                    AuditType,
                    "Partial Success ordering prescription: Attempted to order course ids: {0}, Successful course ids: {1}, Unsuccessful course ids: {2}",
                    _courseIds,
                    string.Join(',', result.Response.SuccessfulOrders.Select(x => x.CourseId)),
                    string.Join(',', result.Response.UnsuccessfulOrders.Select(x => x.CourseId))
                    );
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(OrderPrescriptionResult.PartialSuccess)}");
            }
        }
    }
}
