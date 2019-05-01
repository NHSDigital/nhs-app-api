using System;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.Questionnaire
{
    public interface IQuestionnaireConfig
    {
        Uri BaseCdsUrl { get; }
    }
}