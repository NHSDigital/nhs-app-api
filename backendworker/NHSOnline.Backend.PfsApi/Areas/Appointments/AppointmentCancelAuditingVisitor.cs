using System;
using NHSOnline.Backend.GpSystems.Appointments;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Auditing;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace NHSOnline.Backend.PfsApi.Areas.Appointments
{
    public class AppointmentCancelAuditingVisitor : IAppointmentCancelResultVisitor<Task>
    {
        private readonly IAuditor _auditor;
        private readonly ILogger<AppointmentsController> _logger;
        private readonly string _appointmentId;
        private const string AuditType = Constants.AuditingTitles.CancelAppointmentAuditTypeResponse;

        public AppointmentCancelAuditingVisitor(IAuditor auditor, ILogger<AppointmentsController> logger, string appointmentId)
        {
            _auditor = auditor;
            _logger = logger;
            _appointmentId = appointmentId;
        }

        public async Task Visit(AppointmentCancelResult.Success result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Appointment successfully cancelled for appointment with id: {0}",
                    _appointmentId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentCancelResult.Success)}");
            }
        }

        public async Task Visit(AppointmentCancelResult.BadRequest result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to cancel appointment due to a bad request for appointment with id: {0}",
                    _appointmentId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentCancelResult.BadRequest)}");
            }
        }

        public async Task Visit(AppointmentCancelResult.AppointmentNotCancellable result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to cancel appointment due to it not being cancellable appointment " +
                                          "with id: {0}", _appointmentId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentCancelResult.AppointmentNotCancellable)}");
            }
        }

        public async Task Visit(AppointmentCancelResult.TooLateToCancel result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to cancel appointment due to it being too late to cancel with id: {0}",
                    _appointmentId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentCancelResult.TooLateToCancel)}");
            }
        }

        public async Task Visit(AppointmentCancelResult.Forbidden result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to cancel appointment due to insufficient permissions for appointment " +
                                          "with id: {0}", _appointmentId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentCancelResult.Forbidden)}");
            }
        }

        public async Task Visit(AppointmentCancelResult.BadGateway result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to cancel appointment due to unavailable supplier for appointment " +
                                          "with id: {0}", _appointmentId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentCancelResult.BadGateway)}");
            }
        }

        public async Task Visit(AppointmentCancelResult.InternalServerError result)
        {
            try
            {
                await _auditor.Audit(AuditType, "Unable to cancel appointment due to internal server error for appointment " +
                                                "with id: {0}", _appointmentId);
            }
            catch (Exception e)
            {
                _logger.LogError(e, $"Exception thrown auditing {AuditType} {nameof(AppointmentCancelResult.InternalServerError)}");
            }
        }
    }
}
