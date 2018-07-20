using NHSOnline.Backend.Worker.GpSystems.Appointments;
using NHSOnline.Backend.Worker.Support.Auditing;

namespace NHSOnline.Backend.Worker.Areas.Appointments
{
    public class AppointmentSlotsAuditingVisitor : IAppointmentSlotsResultVisitor<object>
    {
        private readonly IAuditor _auditor;
        private const string AuditType = Constants.AuditingTitles.GetSlotsAuditTypeResponse;

        public AppointmentSlotsAuditingVisitor(IAuditor auditor)
        {
            _auditor = auditor;
        }

        public object Visit(AppointmentSlotsResult.SuccessfullyRetrieved result)
        {
            _auditor.Audit(AuditType, "Available appointment slots successfully viewed");

            return null;
        }

        public object Visit(AppointmentSlotsResult.BadRequest result)
        {
            _auditor.Audit(AuditType, "Available appointment slots view unsuccessful due to bad request");

            return null;
        }

        public object Visit(AppointmentSlotsResult.SupplierSystemUnavailable result)
        {
            _auditor.Audit(AuditType, "Available appointment slots view unsuccessful due to supplier unavailable");

            return null;
        }

        public object Visit(AppointmentSlotsResult.InternalServerError result)
        {
            _auditor.Audit(AuditType, "Available appointment slots view unsuccessful due to internal server error");

            return null;
        }

        public object Visit(AppointmentSlotsResult.CannotBookAppointments result)
        {
            _auditor.Audit(AuditType,  "Available appointment slots view unsuccessful due to not having permissions " +
                                       "to book appointments");

            return null;
        }
    }
}
