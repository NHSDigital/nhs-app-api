using System.Collections.Generic;
using System.Globalization;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir;
using NHSOnline.Backend.Support.ResponseParsers;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public class QuestionnaireClient : IQuestionnaireClient
    {
        
        private readonly QuestionnaireHttpClient _httpClient;
        private readonly ILogger<QuestionnaireClient> _logger;
        private readonly IJsonResponseParser _responseParser;

        private const string QuestionnairePath = "fhir/Questionnaire/{0}?_format=json";

        public QuestionnaireClient(ILogger<QuestionnaireClient> logger, QuestionnaireHttpClient httpClient,
            IJsonResponseParser responseParser)
        {
            _httpClient = httpClient;
            _logger = logger;
            _responseParser = responseParser;
        }

        public async Task<FhirApiQuestionnaireResponse<FhirQuestionnaire>> GetQuestionnaireById(string id)
        {
            var path = string.Format(CultureInfo.InvariantCulture, QuestionnairePath, id);
            var requestMessage = BuildHttpRequest(HttpMethod.Get, path);

            var result = await _httpClient.Client.GetAsync(requestMessage.RequestUri);
            
            var response = new FhirApiQuestionnaireResponse<FhirQuestionnaire>(result.StatusCode);
            return await response.Parse(result, _responseParser, _logger);
        }
        
        private static HttpRequestMessage BuildHttpRequest(HttpMethod httpMethod, string path)
        {
            var request = new HttpRequestMessage(httpMethod, path); 
            return request;
        }
    }
}