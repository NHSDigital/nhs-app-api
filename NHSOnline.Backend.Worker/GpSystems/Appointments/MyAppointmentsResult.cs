using NHSOnline.Backend.Worker.Areas.Appointments.Models;

namespace NHSOnline.Backend.Worker.GpSystems.Appointments
{
    public abstract class MyAppointmentsResult
    {
        private MyAppointmentsResult()
        {
        }

        public abstract T Accept<T>(IMyAppointmentsResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : MyAppointmentsResult
        {
            public MyAppointmentsResponse Response { get; }

            public SuccessfullyRetrieved(MyAppointmentsResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IMyAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadRequest : MyAppointmentsResult
        {
            public override T Accept<T>(IMyAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierSystemUnavailable : MyAppointmentsResult
        {
            public override T Accept<T>(IMyAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : MyAppointmentsResult
        {
            public override T Accept<T>(IMyAppointmentsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
