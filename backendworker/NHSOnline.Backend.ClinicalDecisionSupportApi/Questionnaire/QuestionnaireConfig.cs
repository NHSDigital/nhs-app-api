using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public class QuestionnaireConfig : IQuestionnaireConfig
    {
        public Uri BaseCdsUrl { get; }
        
        public QuestionnaireConfig(IConfiguration configuration, ILogger<QuestionnaireConfig> logger)
        {
            var baseCdsUrl = configuration.GetOrWarn("EMS_CDSS_STUB_URL", logger);
          
            if (!string.IsNullOrEmpty(baseCdsUrl))
            {
                BaseCdsUrl = new Uri(baseCdsUrl, UriKind.Absolute);
            }
        }
    }
}