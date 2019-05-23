using NHSOnline.Backend.ClinicalDecisionSupportApi.ServiceDefinition.Models;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.ServiceDefinition
{
    public interface IServiceDefinitionResultVisitor<out T>
    {
        T Visit(ServiceDefinitionResult.Success result);
        T Visit(ServiceDefinitionResult.InternalServerError result);
        T Visit(ServiceDefinitionResult.BadRequest result);
        T Visit(ServiceDefinitionResult.BadGateway result);
        T Visit(ServiceDefinitionResult.NotFound result);
    }
}