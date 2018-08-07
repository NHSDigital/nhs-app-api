using Microsoft.AspNetCore.Mvc;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentCancelAuditingVisitor : IAppointmentCancelResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private readonly string _appointmentId;
        private const string AuditType = Constants.AuditingTitles.CancelAppointmentAuditTypeResponse;

        public AppointmentCancelAuditingVisitor(IAuditor auditor, string appointmentId)
        {
            _auditor = auditor;
            _appointmentId = appointmentId;
        }

        public object Visit(AppointmentCancelResult.SuccessfullyCancelled successfullyCancelled)
        {
            _auditor.Audit(AuditType, "Appointment successfully cancelled for appointment with id: {0}",
                _appointmentId);

            return null;
        }

        public object Visit(AppointmentCancelResult.BadRequest badRequest)
        {
            _auditor.Audit(AuditType, "Unable to cancel appointment due to a bad request for appointment with id: {0}",
                    _appointmentId);

            return null;
        }

        public object Visit(AppointmentCancelResult.AppointmentNotCancellable appointmentNotCancellable)
        {
            _auditor.Audit(AuditType, "Unable to cancel appointment due to it not being cancellable appointment " +
                                      "with id: {0}", _appointmentId);

            return null;
        }

        public object Visit(AppointmentCancelResult.TooLateToCancel tooLateToCancel)
        {
            _auditor.Audit(AuditType, "Unable to cancel appointment due to it being too late to cancel with id: {0}",
                _appointmentId);

            return null;
        }

        public object Visit(AppointmentCancelResult.InsufficientPermissions insufficientPermissions)
        {
            _auditor.Audit(AuditType, "Unable to cancel appointment due to insufficent permissions for appointment" +
                                      "with id: {0}", _appointmentId);

            return null;
        }

        public object Visit(AppointmentCancelResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            _auditor.Audit(AuditType, "Unable to cancel appointment due to unavailable supplier for appointment " +
                                      "with id: {0}", _appointmentId);

            return null;
        }
    }
}
