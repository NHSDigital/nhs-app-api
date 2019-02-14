using NHSOnline.Backend.GpSystems.Appointments.Models;
using NHSOnline.Backend.GpSystems.SharedModels;

namespace NHSOnline.Backend.GpSystems.Appointments
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

            public Necessity? BookingReasonNecessity { get; }

            public SuccessfullyRetrieved(AppointmentsResponse response, Necessity? bookingReasonNecessity = null)
            {
                Response = response;
                BookingReasonNecessity = bookingReasonNecessity;
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
