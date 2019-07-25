using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public interface IServiceDefinitionResultVisitor<out T>
    {
        T Visit(ServiceDefinitionResult.Success result);
        T Visit(ServiceDefinitionResult.InternalServerError result);
        T Visit(ServiceDefinitionResult.BadRequest result);
        T Visit(ServiceDefinitionResult.BadGateway result);
        T Visit(ServiceDefinitionResult.NotFound result);
        T Visit(ServiceDefinitionResult.DemographicsBadGateway result);
        T Visit(ServiceDefinitionResult.DemographicsRetrievalFailed result);
        T Visit(ServiceDefinitionResult.DemographicsForbidden result);
        T Visit(ServiceDefinitionResult.DemographicsInternalServerError result);
    }
}