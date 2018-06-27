using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public abstract class AppointmentsResult
    {
        private AppointmentsResult()
        {
        }

        public abstract T Accept<T>(IAppointmentsResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : AppointmentsResult
        {
            public AppointmentsResponse Response { get; }

            public SuccessfullyRetrieved(AppointmentsResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : AppointmentsResult
        {
            public override T Accept<T>(IAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : AppointmentsResult
        {
            public override T Accept<T>(IAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : AppointmentsResult
        {
            public override T Accept<T>(IAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class CannotViewAppointments : AppointmentsResult
        {
            public override T Accept<T>(IAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
