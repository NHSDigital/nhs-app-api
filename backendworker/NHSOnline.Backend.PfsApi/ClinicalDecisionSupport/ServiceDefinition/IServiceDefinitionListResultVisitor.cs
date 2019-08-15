using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public interface IServiceDefinitionListResultVisitor<out T>
    {
        T Visit(ServiceDefinitionListResult.Success result);
        T Visit(ServiceDefinitionListResult.InternalServerError result);
        T Visit(ServiceDefinitionListResult.BadRequest result);
        T Visit(ServiceDefinitionListResult.BadGateway result);
        T Visit(ServiceDefinitionListResult.NotFound result);
    }
}