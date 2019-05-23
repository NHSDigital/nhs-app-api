namespace NHSOnline.Backend.GpSystems.Appointments
{
    public abstract class AppointmentCancelResult
    {
        public abstract T Accept<T>(IAppointmentCancelResultVisitor<T> visitor);

        public class Success : AppointmentCancelResult
        {
            public override T Accept<T>( IAppointmentCancelResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : AppointmentCancelResult
        {
            public override T Accept<T>(IAppointmentCancelResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : AppointmentCancelResult
        {
            public override T Accept<T>(IAppointmentCancelResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class AppointmentNotCancellable : AppointmentCancelResult
        {
            public override T Accept<T>(IAppointmentCancelResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class TooLateToCancel : AppointmentCancelResult
        {
            public override T Accept<T>(IAppointmentCancelResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : AppointmentCancelResult
        {
            public override T Accept<T>(IAppointmentCancelResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : AppointmentCancelResult
        {
            public override T Accept<T>(IAppointmentCancelResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
