using System.Net;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.Http;

namespace NHSOnline.Backend.ServiceJourneyRules.Common.Models
{
    public abstract class ServiceJourneyRulesApiResponse: ApiResponse
    {
        protected ServiceJourneyRulesApiResponse(HttpStatusCode statusCode) :base(statusCode)
        {}

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();

        public override bool HasBadRequestResponse => StatusCode.IsBadRequestCode();
            
        public override string  ErrorForLogging => $"Error Code: '{StatusCode}'. ";
    }
}