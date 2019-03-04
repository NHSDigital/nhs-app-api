using System;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support.Auditing;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentBookAuditingVisitor : IAppointmentBookResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly string _slotId;
        private readonly DateTimeOffset? _slotStartTime;
        private const string AuditType = Constants.AuditingTitles.BookAppointmentAuditTypeResponse;

        public AppointmentBookAuditingVisitor(IAuditor auditor, ILogger<AppointmentsController> logger, string slotId, DateTimeOffset? slotStartTime)
        {
            _auditor = auditor;
            _logger = logger;
            _slotId = slotId;
            _slotStartTime = slotStartTime;
        }

        public async Task Visit(AppointmentBookResult.SuccessfullyBooked successfullyBooked)
        {
            try
            {
                await _auditor.Audit(AuditType, "Appointment successfully booked for appointment with id: {0} and startDateTime: {1:O}",
                    _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.SuccessfullyBooked)}");
            }
        }

        public async Task Visit(AppointmentBookResult.InsufficientPermissions insufficientPermissions)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to insufficent permissions for appointment " +
                                          "with id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.InsufficientPermissions)}");
            }
        }

        public async Task Visit(AppointmentBookResult.SlotNotAvailable slotNotAvailable)
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

        public async Task Visit(AppointmentBookResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to unavailable supplier for appointment with " +
                                          "id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.SupplierSystemUnavailable)}");
            }
        }

        public async Task Visit(AppointmentBookResult.BadRequest badRequest)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due to bad request for appointment with id: {0}" +
                                          "and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.BadRequest)}");
            }
        }

        public async Task Visit(AppointmentBookResult.AppointmentLimitReached appointmentLimitReached)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due appointment limit reached for appointment " +
                                          "with id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.AppointmentLimitReached)}");
            }
        }

        public async Task Visit(AppointmentBookResult.InternalServerError internalServerError)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to book appointment due appointment limit reached for appointment " +
                                                "with id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentBookResult.InternalServerError)}");
            }
        }
    }
}
