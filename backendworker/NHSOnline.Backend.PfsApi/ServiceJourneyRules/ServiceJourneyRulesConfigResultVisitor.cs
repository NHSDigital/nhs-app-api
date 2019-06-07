using Microsoft.AspNetCore.Http;
using NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models;
using NHSOnline.Backend.ServiceJourneyRulesApi.Models;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules
{
    internal class ServiceJourneyRulesConfigResultVisitor : IServiceJourneyRulesConfigResultVisitor<ServiceJourneyRulesVisitorOutput>
    {
        public ServiceJourneyRulesVisitorOutput Visit(ServiceJourneyRulesConfigResult.Success result)
        {
            return new ServiceJourneyRulesVisitorOutput
            {
                ServiceJourneyRulesRetrieved = true,
                Response = result.Response
            };
        }

        public ServiceJourneyRulesVisitorOutput Visit(ServiceJourneyRulesConfigResult.NotFound result)
        {
            return new ServiceJourneyRulesVisitorOutput
            {
                ServiceJourneyRulesRetrieved = false,
                StatusCode = StatusCodes.Status404NotFound
            };
        }

        public ServiceJourneyRulesVisitorOutput Visit(ServiceJourneyRulesConfigResult.BadGateway result)
        {
            return new ServiceJourneyRulesVisitorOutput
            {
                ServiceJourneyRulesRetrieved = false,
                StatusCode = StatusCodes.Status502BadGateway
            };
        }
    }
}