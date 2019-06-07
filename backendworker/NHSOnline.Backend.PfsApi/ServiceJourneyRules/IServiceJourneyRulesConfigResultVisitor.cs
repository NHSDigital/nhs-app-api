using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    public interface IServiceJourneyRulesConfigResultVisitor<out T>
    {
        T Visit(ServiceJourneyRulesConfigResult.Success result);
        T Visit(ServiceJourneyRulesConfigResult.NotFound result);
        T Visit(ServiceJourneyRulesConfigResult.BadGateway result);
    }
}