namespace NHSOnline.Backend.GpSystems.Demographics
{
    public abstract class DemographicsResult
    {
        private DemographicsResult()
        {
        }
        
        public abstract T Accept<T>(IDemographicsResultVisitor<T> visitor);
        
        public class Success : DemographicsResult
        {
            public DemographicsResponse Response { get; }

            public Success(DemographicsResponse response)
            {
                Response = response;
            }
            
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadGateway : DemographicsResult
        {
            public override T Accept<T>(IDemographicsResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class Forbidden : DemographicsResult
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