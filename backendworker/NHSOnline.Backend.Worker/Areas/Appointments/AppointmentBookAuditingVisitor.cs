using System;
using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentBookAuditingVisitor : IAppointmentBookResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private readonly string _slotId;
        private readonly DateTimeOffset? _slotStartTime;
        private const string AuditType = Constants.AuditingTitles.BookAppointmentAuditTypeResponse;

        public AppointmentBookAuditingVisitor(IAuditor auditor, string slotId, DateTimeOffset? slotStartTime)
        {
            _auditor = auditor;
            _slotId = slotId;
            _slotStartTime = slotStartTime;
        }

        public object Visit(AppointmentBookResult.SuccessfullyBooked successfullyBooked)
        {
            _auditor.Audit(AuditType, "Appointment successfully booked for appointment with id: {0} and startDateTime: {1:O}",
                    _slotId, _slotStartTime);

            return null;
        }

        public object Visit(AppointmentBookResult.InsufficientPermissions insufficientPermissions)
        {
            _auditor.Audit(AuditType, "Unable to book appointment due to insufficent permissions for appointment " +
                                      "with id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);

            return null;
        }

        public object Visit(AppointmentBookResult.SlotNotAvailable slotNotAvailable)
        {
            _auditor.Audit(AuditType, "Unable to book appointment due to appointment being unavailable for appointment with " +
                              "id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);

            return null;
        }

        public object Visit(AppointmentBookResult.SupplierSystemUnavailable supplierSystemUnavailable)
        {
            _auditor.Audit(AuditType, "Unable to book appointment due to unavailable supplier for appointment with " +
                                      "id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);

            return null;
        }

        public object Visit(AppointmentBookResult.BadRequest badRequest)
        {
            _auditor.Audit(AuditType, "Unable to book appointment due to bad request for appointment with id: {0}" +
                              "and startDateTime: {1:O}", _slotId, _slotStartTime);

            return null;
        }

        public object Visit(AppointmentBookResult.AppointmentLimitReached appointmentLimitReached)
        {
            _auditor.Audit(AuditType, "Unable to book appointment due appointment limit reached for appointment " +
                                      "with id: {0} and startDateTime: {1:O}", _slotId, _slotStartTime);

            return null;
        }
    }
}
