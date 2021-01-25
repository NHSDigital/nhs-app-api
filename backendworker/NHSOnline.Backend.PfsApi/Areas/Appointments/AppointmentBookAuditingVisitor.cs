using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Auditing;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Metrics;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentBookAuditingVisitor : IAppointmentBookResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly IMetricLogger _metricLogger;
        private readonly P9UserSession _userSession;
        private readonly string _slotId;
        private readonly DateTimeOffset? _slotStartTime;
        private const string AuditType = AuditingOperations.BookAppointmentAuditTypeResponse;

        public AppointmentBookAuditingVisitor(IAuditor auditor, ILogger<AppointmentsController> logger, string slotId, DateTimeOffset? slotStartTime, IMetricLogger metricLogger,
            P9UserSession userSession)
        {
            _auditor = auditor;
            _logger = logger;
            _slotId = slotId;
            _slotStartTime = slotStartTime;
            _metricLogger = metricLogger;
            _userSession = userSession;
        }

        public async Task Visit(AppointmentBookResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Appointment successfully booked for appointment with id: {0} and startDateTime: {1:O}",
                    _slotId, _slotStartTime);
                await _metricLogger.AppointmentBook(new AppointmentData(_userSession.Key));
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.Success)}");
            }
        }

        public async Task Visit(AppointmentBookResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to insufficient permissions for appointment " +
                                          "with id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.Forbidden)}");
            }
        }

        public async Task Visit(AppointmentBookResult.SlotNotAvailable result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to appointment being unavailable for appointment with " +
                                          "id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.SlotNotAvailable)}");
            }
        }

        public async Task Visit(AppointmentBookResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to unavailable supplier for appointment with " +
                                          "id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.BadGateway)}");
            }
        }

        public async Task Visit(AppointmentBookResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to bad request for appointment with id: {0}" +
                                          " and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.BadRequest)}");
            }
        }

        public async Task Visit(AppointmentBookResult.AppointmentLimitReached result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to appointment limit reached for appointment " +
                                          "with id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.AppointmentLimitReached)}");
            }
        }

        public async Task Visit(AppointmentBookResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to internal server error for appointment " +
                                                "with id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.InternalServerError)}");
            }
        }
    }
}
