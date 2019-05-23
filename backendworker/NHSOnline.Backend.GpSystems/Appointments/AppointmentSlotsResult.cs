using NHSOnline.Backend.GpSystems.Appointments.Models;

namespace NHSOnline.Backend.GpSystems.Appointments
{
    public abstract class AppointmentSlotsResult
    {
        private AppointmentSlotsResult()
        {
        }

        public abstract T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor);
        
        public class Success : AppointmentSlotsResult
        {
            public AppointmentSlotsResponse Response { get; }

            public Success(AppointmentSlotsResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadGateway : AppointmentSlotsResult
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

        public class Forbidden : AppointmentSlotsResult
        {
            public override T Accept<T>(IAppointmentSlotsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
