namespace NHSOnline.Backend.GpSystems.Appointments
{
    public abstract class AppointmentBookResult
    {
        private AppointmentBookResult()
        {
        }

        public abstract T Accept<T>(IAppointmentBookResultVisitor<T> visitor);

        public class Success : AppointmentBookResult
        {
            public override T Accept<T>(IAppointmentBookResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : AppointmentBookResult
        {
            public override T Accept<T>(IAppointmentBookResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SlotNotAvailable : AppointmentBookResult
        {
            public override T Accept<T>(IAppointmentBookResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : AppointmentBookResult
        {
            public override T Accept<T>(IAppointmentBookResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : AppointmentBookResult
        {
            public override T Accept<T>(IAppointmentBookResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class AppointmentLimitReached : AppointmentBookResult
        {
            public override T Accept<T>(IAppointmentBookResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : AppointmentBookResult
        {
            public override T Accept<T>(IAppointmentBookResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
