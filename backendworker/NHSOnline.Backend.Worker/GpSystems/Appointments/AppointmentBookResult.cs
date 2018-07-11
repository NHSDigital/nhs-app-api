namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public abstract class AppointmentBookResult
    {
        private AppointmentBookResult()
        {
        }

        public abstract T Accept<T>(IAppointmentBookResultVisitor<T> visitor);

        public class SuccessfullyBooked : AppointmentBookResult
        {
            public override T Accept<T>(IAppointmentBookResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InsufficientPermissions : AppointmentBookResult
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

        public class SupplierSystemUnavailable : AppointmentBookResult
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
    }
}