using System.Collections.Generic;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models
{
    public abstract class ServiceDefinitionListResult
    {
        public abstract T Accept<T>(IServiceDefinitionListResultVisitor<T> visitor);

        public class Success : ServiceDefinitionListResult
        {
            public List<ServiceDefinitionCategory> Response { get; }

            public Success(List<ServiceDefinitionCategory> response)
            {
                Response = response;
            }

            public override T Accept<T>(IServiceDefinitionListResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class InternalServerError : ServiceDefinitionListResult
        {
            public override T Accept<T>(IServiceDefinitionListResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
        
        public class BadRequest : ServiceDefinitionListResult
        {
            public override T Accept<T>(IServiceDefinitionListResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class BadGateway : ServiceDefinitionListResult
        {
            public override T Accept<T>(IServiceDefinitionListResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }

        public class NotFound : ServiceDefinitionListResult
        {
            public override T Accept<T>(IServiceDefinitionListResultVisitor<T> visitor)
            {
                return visitor.Visit(this);
            }
        }
    }
}
