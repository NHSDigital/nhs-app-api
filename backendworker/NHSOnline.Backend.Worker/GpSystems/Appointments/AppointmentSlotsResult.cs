using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public abstract class AppointmentSlotsResult
    {
        private AppointmentSlotsResult()
        {
        }

        public abstract T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor);
        
        public class SuccessfullyRetrieved : AppointmentSlotsResult
        {
            public AppointmentSlotsResponse Response { get; }

            public SuccessfullyRetrieved(AppointmentSlotsResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : AppointmentSlotsResult
        {
            public override T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierSystemUnavailable : AppointmentSlotsResult
        {
            public override T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : AppointmentSlotsResult
        {
            public override T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class CannotBookAppointments : AppointmentSlotsResult
        {
            public override T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
