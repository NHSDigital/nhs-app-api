using NHSOnline.Backend.Worker.GpSystems.Suppliers.Emis.Models.Demographics;

namespace NHSOnline.Backend.Worker.GpSystems.Demographics
{
    public abstract class GetDemographicsResult
    {
        private GetDemographicsResult()
        {
        }
        
        public abstract T Accept<T>(IDemographicsResultVisitor<T> visitor);
        
        public class SuccessfullyRetrieved : GetDemographicsResult
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
        
        public class SupplierSystemUnavailable : GetDemographicsResult
        {
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class UserHasNoAccess : GetDemographicsResult
        {
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Unsuccessful : GetDemographicsResult
        {
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}