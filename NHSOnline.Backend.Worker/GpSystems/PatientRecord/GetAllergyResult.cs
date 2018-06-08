using NHSOnline.Backend.Worker.Areas.MyRecord.Models;

namespace NHSOnline.Backend.Worker.GpSystems.PatientRecord
{
    public abstract class GetAllergyResult
    {
        public abstract T Accept<T>(IAllergyResultVisitor<T> visitor);

        public class SuccessfullyRetrieved : GetAllergyResult
        {
            public AllergyListResponse Response { get; }

            public SuccessfullyRetrieved(AllergyListResponse response)
            {
                Response = response;
            }

            public override T Accept<T>(IAllergyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Unsuccessful : GetAllergyResult
        {
            public override T Accept<T>(IAllergyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class UserHasNoAccess : GetAllergyResult
        {
            public override T Accept<T>(IAllergyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class SupplierBadData : GetAllergyResult
        {
            public override T Accept<T>(IAllergyResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
