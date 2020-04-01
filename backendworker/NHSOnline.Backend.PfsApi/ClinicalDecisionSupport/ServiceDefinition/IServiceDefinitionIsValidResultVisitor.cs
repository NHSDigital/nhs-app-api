using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public interface IServiceDefinitionIsValidResultVisitor<out T>
    {
        T Visit(ServiceDefinitionIsValidResult.Valid result);
        T Visit(ServiceDefinitionIsValidResult.Invalid result);
        T Visit(ServiceDefinitionIsValidResult.BadRequest result);
        T Visit(ServiceDefinitionIsValidResult.BadGateway result);
    }
}