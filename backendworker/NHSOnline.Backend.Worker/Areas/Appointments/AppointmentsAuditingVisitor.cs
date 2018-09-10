using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentsAuditingVisitor : IAppointmentsResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.ViewAppointmentAuditTypeResponse;

        public AppointmentsAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }

        public object Visit(AppointmentsResult.SuccessfullyRetrieved result)
        {
            _auditor.Audit(AuditType, "Booked appointments successfully viewed");

            return null;
        }

        public object Visit(AppointmentsResult.BadRequest result)
        {
            _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to bad request");

            return null;
        }

        public object Visit(AppointmentsResult.SupplierSystemUnavailable result)
        {
            _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to supplier being unavailable");

            return null;
        }

        public object Visit(AppointmentsResult.InternalServerError result)
        {
            _auditor.Audit(AuditType, "Booked appointments view unsuccessful due to internal server error");

            return null;
        }

        public object Visit(AppointmentsResult.CannotViewAppointments result)
        {
            _auditor.Audit(AuditType, "Booked appointments view unsuccessful");

            return null;
        }
    }
}
