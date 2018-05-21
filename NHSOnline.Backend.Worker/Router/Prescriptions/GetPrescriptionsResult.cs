using NHSOnline.Backend.Worker.Areas.Prescriptions.Models;

namespace NHSOnline.Backend.Worker.Router.Prescriptions
{
    public abstract class GetPrescriptionsResult
    {
        public abstract T Accept<T>(IPrescriptionResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetPrescriptionsResult
        {
            public PrescriptionListResponse Response { get; }

            public SuccessfullyRetrieved(PrescriptionListResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : GetPrescriptionsResult
        {
            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierBadData : GetPrescriptionsResult
        {
            public override T Accept<T>(IPrescriptionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
