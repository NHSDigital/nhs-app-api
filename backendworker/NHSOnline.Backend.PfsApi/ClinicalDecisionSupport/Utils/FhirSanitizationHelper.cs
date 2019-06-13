using System;
using System.Collections.Generic;
using System.Linq;
using Hl7.Fhir.Model;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils
{
    public class FhirSanitizationHelper : IFhirSanitizationHelper
    {
        public void SanitizeGuidanceResponse(GuidanceResponse guidanceResponse, IHtmlSanitizer htmlSanitizer)
        {
            if (guidanceResponse == null) return;

            SanitizeGuidanceResponseResults(guidanceResponse, htmlSanitizer);
            SanitizeQuestionnaires(guidanceResponse, guidanceResponse.DataRequirement, htmlSanitizer);
        }
        
        public void SanitizeServiceDefinition(Hl7.Fhir.Model.ServiceDefinition serviceDefinition, IHtmlSanitizer htmlSanitizer)
        {
            if (serviceDefinition == null) return;

            SanitizeQuestionnaires(serviceDefinition, serviceDefinition.DataRequirement, htmlSanitizer);
        }

        public void SanitizeServiceDefinitionSearchBundle(Bundle bundle, IHtmlSanitizer htmlSanitizer)
        {
            var serviceDefinitionEntries = bundle.Entry
                .Where(e => e.Resource.ResourceType == ResourceType.ServiceDefinition)
                .Select(e => e.Resource as Hl7.Fhir.Model.ServiceDefinition).ToList();

            foreach (var serviceDefinition in serviceDefinitionEntries)
            {
                SanitizeServiceDefinition(serviceDefinition, htmlSanitizer);
            }
        }
        
        private static void SanitizeGuidanceResponseResults(GuidanceResponse guidanceResponse, IHtmlSanitizer htmlSanitizer)
        {
            var resultId = guidanceResponse.Result?.Reference?.Substring(1);

            if (string.IsNullOrWhiteSpace(resultId)) return;

            var referralRequestAndCarePlanIds = GetReferralRequestAndCarePlanIds(guidanceResponse, resultId).ToList();
            
            if (referralRequestAndCarePlanIds.Count == 0) return;

            foreach (var contained in guidanceResponse.Contained)
            {
                if (!referralRequestAndCarePlanIds.Contains(contained.Id)) continue;

                var carePlan = contained as CarePlan;
                var referralRequest = contained as ReferralRequest;

                SanitizeReferralRequest(referralRequest, htmlSanitizer);
                SanitizeCarePlan(carePlan, htmlSanitizer);
            }
        }
        
        private static IEnumerable<string> GetReferralRequestAndCarePlanIds(DomainResource domainResource, string resultId)
        {
            var result = new List<string>();

            if (domainResource == null || string.IsNullOrWhiteSpace(resultId) || domainResource.Contained == null) return result;

            if (!(domainResource.Contained
                    .FirstOrDefault(contained => resultId.Equals(contained.Id, StringComparison.Ordinal)) is
                RequestGroup requestGroup)) return result;

            foreach (var actionComponent in requestGroup.Action)
            {
                result.Add(actionComponent?.Resource?.Reference?.Substring(1));
            }

            return result;
        }

        private static void SanitizeReferralRequest(ReferralRequest referralRequest, IHtmlSanitizer htmlSanitizer)
        {
            if (referralRequest == null) return;

            referralRequest.Description = htmlSanitizer.SanitizeHtml(referralRequest.Description);
        }

        private static void SanitizeCarePlan(CarePlan carePlan, IHtmlSanitizer htmlSanitizer)
        {
            if (carePlan == null) return;
            
            carePlan.Title = htmlSanitizer.SanitizeHtml(carePlan.Title);
            SanitizeCarePlanActivities(carePlan.Activity, htmlSanitizer);
        }

        private static void SanitizeCarePlanActivities(IReadOnlyCollection<CarePlan.ActivityComponent> activityComponents, IHtmlSanitizer htmlSanitizer)
        {
            if (activityComponents == null) return;

            foreach (var activityComponent in activityComponents)
            {
                activityComponent.Detail.Description = htmlSanitizer.SanitizeHtml(activityComponent.Detail?.Description);
            }
        }
        
        private static void SanitizeQuestionnaires(DomainResource domainResource, IReadOnlyCollection<DataRequirement> dataRequirements, IHtmlSanitizer htmlSanitizer)
        {
            var questionnaireIds = GetQuestionnaireIdsFromDataRequirements(dataRequirements).ToList();

            SanitizeQuestionnaires(domainResource, questionnaireIds, htmlSanitizer);
        }
        
        private static IEnumerable<string> GetQuestionnaireIdsFromDataRequirements(IReadOnlyCollection<DataRequirement> dataRequirements)
        {
            var result = new List<string>();

            if (dataRequirements == null) return result;

            foreach (var dataRequirement in dataRequirements)
            {
                if (dataRequirement.Type != FHIRAllTypes.QuestionnaireResponse) continue;
                
                foreach (var extension in dataRequirement.Extension)
                {
                    result.Add((extension.Value as ResourceReference)?.Reference?.Substring(1));
                }
            }

            return result;
        }

        private static void SanitizeQuestionnaires(DomainResource domainResource,
            IReadOnlyCollection<string> questionnaireIds, IHtmlSanitizer htmlSanitizer)
        {
            foreach (var contained in domainResource.Contained)
            {
                if (!questionnaireIds.Contains(contained.Id)) continue;

                var questionnaire = contained as Questionnaire;
                SanitizeItems(questionnaire?.Item, htmlSanitizer);
            }
        }
        
        private static void SanitizeItems(List<Questionnaire.ItemComponent> items, IHtmlSanitizer htmlSanitizer)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }
            
            items.ForEach(item =>
            {
                item.Text = htmlSanitizer.SanitizeHtml(item.Text);
                
                SanitizeItems(item.Item, htmlSanitizer);
            });
        }
    }
}