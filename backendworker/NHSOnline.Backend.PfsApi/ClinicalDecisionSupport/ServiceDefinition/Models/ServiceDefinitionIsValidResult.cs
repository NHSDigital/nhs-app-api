namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models
{
    public abstract class ServiceDefinitionIsValidResult
    {
        public abstract T Accept<T>(IServiceDefinitionIsValidResultVisitor<T> visitor);

        public class Valid : ServiceDefinitionIsValidResult
        {
            public override T Accept<T>(IServiceDefinitionIsValidResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class Invalid : ServiceDefinitionIsValidResult
        {
            public override T Accept<T>(IServiceDefinitionIsValidResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : ServiceDefinitionIsValidResult
        {
            public override T Accept<T>(IServiceDefinitionIsValidResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : ServiceDefinitionIsValidResult
        {
            public override T Accept<T>(IServiceDefinitionIsValidResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}