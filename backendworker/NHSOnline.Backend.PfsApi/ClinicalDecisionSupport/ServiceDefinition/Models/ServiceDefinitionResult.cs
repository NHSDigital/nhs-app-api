
using Microsoft.AspNetCore.Mvc.Versioning;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models
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
        
        public class CustomError : ServiceDefinitionResult
        {
            public int ErrorCode  { get; }

            public CustomError(int code)
            {
                ErrorCode = code;
            }

            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor)
            {
                return visitor.Visit(this, ErrorCode);
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
        
        public class DemographicsBadGateway : ServiceDefinitionResult
        {
            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsRetrievalFailed : ServiceDefinitionResult
        {
            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsForbidden : ServiceDefinitionResult
        {
            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor) => visitor.Visit(this);
        }

        public class DemographicsInternalServerError : ServiceDefinitionResult
        {
            public override T Accept<T>(IServiceDefinitionResultVisitor<T> visitor) => visitor.Visit(this);
        }
    }
}
