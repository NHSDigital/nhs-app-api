namespace NHSOnline.Backend.GpSystems.Demographics
{
    public abstract class DemographicsResult
    {
        private DemographicsResult()
        {
        }
        
        public abstract T Accept<T>(IDemographicsResultVisitor<T> visitor);
        
        public class SuccessfullyRetrieved : DemographicsResult
        {
            public DemographicsResponse Response { get; }

            public SuccessfullyRetrieved(DemographicsResponse response)
            {
                Response = response;
            }
            
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class SupplierSystemUnavailable : DemographicsResult
        {
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class UserHasNoAccess : DemographicsResult
        {
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Unsuccessful : DemographicsResult
        {
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class InternalServerError : DemographicsResult
        {
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}