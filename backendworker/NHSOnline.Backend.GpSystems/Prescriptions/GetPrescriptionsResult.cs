using NHSOnline.Backend.GpSystems.Prescriptions.Models;

namespace NHSOnline.Backend.GpSystems.Prescriptions
{
    public abstract class GetPrescriptionsResult
    {   
        public abstract T Accept<T>(IGetPrescriptionsResultVisitor<T> visitor);

        public class Success : GetPrescriptionsResult
        {
            public PrescriptionListResponse Response { get; }
            
            public FilteringCounts FilteringCounts { get; }

            public Success(PrescriptionListResponse response,
                FilteringCounts filteringCounts)
            {
                Response = response;
                FilteringCounts = filteringCounts;
            }

            public override T Accept<T>(IGetPrescriptionsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : GetPrescriptionsResult
        {
            public override T Accept<T>(IGetPrescriptionsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Forbidden : GetPrescriptionsResult
        {
            public override T Accept<T>(IGetPrescriptionsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : GetPrescriptionsResult
        {
            public override T Accept<T>(IGetPrescriptionsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : GetPrescriptionsResult
        {
            public override T Accept<T>(IGetPrescriptionsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}