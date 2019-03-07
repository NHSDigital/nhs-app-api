using System.Net;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models
{
    public abstract class ServiceJourneyRulesApiResponse: ApiResponse
    {
        protected ServiceJourneyRulesApiResponse(HttpStatusCode statusCode) :base(statusCode)
        {}

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();
            
        public override string  ErrorForLogging => $"Error Code: '{StatusCode}'. ";
    }
}