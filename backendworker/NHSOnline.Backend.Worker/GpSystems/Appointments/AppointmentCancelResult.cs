namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public abstract class AppointmentCancelResult
    {
        private AppointmentCancelResult()
        {
        }

        public abstract T Accept<T>(IAppointmentCancelResultVisitor<T> visitor);

        public class SuccessfullyCancelled : AppointmentCancelResult
        {
            public override T Accept<T>(IAppointmentCancelResultVisitor<T> visitor)
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

        public class InsufficientPermissions : AppointmentCancelResult
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

        public class SupplierSystemUnavailable : AppointmentCancelResult
        {
            public override T Accept<T>(IAppointmentCancelResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}