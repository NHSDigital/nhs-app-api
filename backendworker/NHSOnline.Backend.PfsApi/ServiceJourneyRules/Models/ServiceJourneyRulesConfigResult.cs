using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models
{
    public abstract class ServiceJourneyRulesConfigResult
    {   
        public abstract T Accept<T>(IServiceJourneyRulesConfigResultVisitor<T> visitor);

        public class Success : ServiceJourneyRulesConfigResult
        {
            public ServiceJourneyRulesResponse Response { get; }

            public Success(ServiceJourneyRulesResponse response)
            {
                Response = response;
            }
            
            public override T Accept<T>(
                IServiceJourneyRulesConfigResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class NotFound : ServiceJourneyRulesConfigResult
        {
            public override T Accept<T>(
                IServiceJourneyRulesConfigResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            } 
        }
        
        public class BadGateway : ServiceJourneyRulesConfigResult
        {  
            public override T Accept<T>(
                IServiceJourneyRulesConfigResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}