using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.PfsApi.ServiceJourneyRules.Models
{
    public class ServiceJourneyRulesApiObjectResponse<TBody> : ServiceJourneyRulesApiResponse
    {
        public TBody Body { get; set; }
            
        public ServiceJourneyRulesApiObjectResponse(HttpStatusCode statusCode) : base(statusCode)
        {}

        public async Task<ServiceJourneyRulesApiObjectResponse<TBody>> Parse(
            HttpResponseMessage responseMessage,
            IJsonResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);
            return string.IsNullOrEmpty(stringResponse)
                ? this : ParseResponse(responseParser, stringResponse, responseMessage);
        }

        private ServiceJourneyRulesApiObjectResponse<TBody> ParseResponse(
            IResponseParser responseParser, 
            string stringResponse, 
            HttpResponseMessage responseMessage)
        {
            Body = responseParser.ParseBody<TBody>(stringResponse, responseMessage);
            return this;
        }

        protected override bool FormatResponseIfUnsuccessful => true;
    }
}