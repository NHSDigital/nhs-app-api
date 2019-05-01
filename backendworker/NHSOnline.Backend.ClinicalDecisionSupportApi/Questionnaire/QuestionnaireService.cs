using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models;
using NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire.Models.Fhir;
using NHSOnline.Backend.Support.Logging;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public class QuestionnaireService : IQuestionnaireService
    {
        private readonly IQuestionnaireClient _client;
        private readonly IHtmlSanitizer _htmlSanitizer;
        private readonly ILogger<QuestionnaireService> _logger;

        public QuestionnaireService(IQuestionnaireClient client,
            ILogger<QuestionnaireService> logger, IHtmlSanitizer htmlSanitizer)
        {
            _client = client;
            _logger = logger;
            _htmlSanitizer = htmlSanitizer;
        }
        public async Task<QuestionnaireResult> GetQuestionnaireById(string id)
        {
            try
            {
                _logger.LogEnter();
                
                var result = await _client.GetQuestionnaireById(id);

                if (!result.HasSuccessResponse)
                {
                    _logger.LogError(result.ErrorForLogging);
                    return new QuestionnaireResult.Unsuccessful();
                }
                
                var questionnaire = result.Body;

                if (questionnaire == null)
                {
                    _logger.LogError($"Questionnaire {id} response body is null");
                    return new QuestionnaireResult.Unsuccessful();
                }

                SanitizeItems(questionnaire.Item);

                return new QuestionnaireResult.SuccessfullyRetrieved(questionnaire);
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, $"Failed to retrieve Questionnaire from external service: {id} ");
                return new QuestionnaireResult.Unsuccessful();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error processing request for Questionnaire: {id} ");
                return new QuestionnaireResult.Unsuccessful();
            }
            finally
            {
                _logger.LogExit();
            }
        }

        private void SanitizeItems(List<FhirItem> items)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }
            
            items.ForEach(item =>
            {
                item.Text = _htmlSanitizer.SanitizeHtml(item.Text);
                SanitizeItems(item.Item);
            });
        }
    }
}