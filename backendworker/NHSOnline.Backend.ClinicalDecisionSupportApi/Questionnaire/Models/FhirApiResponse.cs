using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.GpSystems;
using NHSOnline.Backend.Support;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models
{
    public abstract class FhirApiResponse: ApiResponse
    {
        protected FhirApiResponse(HttpStatusCode statusCode) : base(statusCode)
        {}

        public override bool HasSuccessResponse => StatusCode.IsSuccessStatusCode();
            
        public override string  ErrorForLogging => $"Error Code: '{StatusCode}'. ";

    }
        
    public class FhirApiQuestionnaireResponse<TBody> : FhirApiResponse
    {
        public TBody Body { get; set; }
        public FhirApiQuestionnaireResponse(HttpStatusCode statusCode) : base(statusCode)
        {}

        public async Task<FhirApiQuestionnaireResponse<TBody>> Parse(
            HttpResponseMessage responseMessage,
            IJsonResponseParser responseParser,
            ILogger logger)
        {
            var stringResponse = await GetStringResponse(responseMessage, logger);
            return string.IsNullOrEmpty(stringResponse)
                ? this : ParseResponse(responseParser, stringResponse, responseMessage);
        }

        private FhirApiQuestionnaireResponse<TBody> ParseResponse(
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