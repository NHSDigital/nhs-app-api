using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.ServiceJourneyRules.Common.Models
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
                ? this : ParseResponse(responseParser, stringResponse);
        }

        private ServiceJourneyRulesApiObjectResponse<TBody> ParseResponse(
            IResponseParser responseParser, 
            string stringResponse)
        {
            Body = responseParser.ParseBody<TBody>(stringResponse);
            return this;
        }

        protected override bool FormatResponseIfUnsuccessful => true;
    }
}