namespace NHSOnline.Backend.ClinicalDecisionSupportApi.ServiceDefinition.Models
{
    public abstract class ServiceDefinitionResult
    {
        public abstract T Accept<T>(IServiceDefinitionResultVisitor<T> visitor);

        public class Success : ServiceDefinitionResult
        {
            public string Response { get; }

            public Success(string response)
            {
                Response = response;
            }

            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : ServiceDefinitionResult
        {
            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : ServiceDefinitionResult
        {
            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : ServiceDefinitionResult
        {
            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : ServiceDefinitionResult
        {
            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
