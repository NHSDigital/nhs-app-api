using System.Net.Http;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public class QuestionnaireHttpClient
    {
        public HttpClient Client { get; }
        
        public QuestionnaireHttpClient(HttpClient client, IQuestionnaireConfig config)
        {
            client.BaseAddress = config.BaseCdsUrl;
            Client = client;
        }
    }
}