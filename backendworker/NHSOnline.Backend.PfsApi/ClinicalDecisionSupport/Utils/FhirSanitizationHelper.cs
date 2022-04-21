extern alias stu3;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Hl7.Fhir.Model;
using NHSOnline.Backend.Support.Sanitization;
using STU3Models = stu3::Hl7.Fhir.Model;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils
{
    public class FhirSanitizationHelper : IFhirSanitizationHelper
    {
        public void SanitizeGuidanceResponse(STU3Models.GuidanceResponse guidanceResponse, IHtmlSanitizer htmlSanitizer)
        {
            if (guidanceResponse == null)
            {
                return;
            }

            SanitizeGuidanceResponseResults(guidanceResponse, htmlSanitizer);
            SanitizeQuestionnaires(guidanceResponse, guidanceResponse.DataRequirement, htmlSanitizer);
        }

        public void SanitizeServiceDefinition(STU3Models.ServiceDefinition serviceDefinition, IHtmlSanitizer htmlSanitizer)
        {
            if (serviceDefinition == null)
            {
                return;
            }

            SanitizeQuestionnaires(serviceDefinition, serviceDefinition.DataRequirement, htmlSanitizer);
        }

        private static void SanitizeGuidanceResponseResults(STU3Models.GuidanceResponse guidanceResponse, IHtmlSanitizer htmlSanitizer)
        {
            var resultId = guidanceResponse.Result?.Reference?.Substring(1);

            if (string.IsNullOrWhiteSpace(resultId))
            {
                return;
            }

            var referralRequestAndCarePlanIds = GetReferralRequestAndCarePlanIds(guidanceResponse, resultId).ToList();

            if (referralRequestAndCarePlanIds.Count == 0)
            {
                return;
            }

            foreach (var contained in guidanceResponse.Contained)
            {
                if (!referralRequestAndCarePlanIds.Contains(contained.Id))
                {
                    continue;
                }

                var carePlan = contained as STU3Models.CarePlan;
                var referralRequest = contained as STU3Models.ReferralRequest;

                SanitizeReferralRequest(referralRequest, htmlSanitizer);
                SanitizeCarePlan(carePlan, htmlSanitizer);
            }
        }

        private static IEnumerable<string> GetReferralRequestAndCarePlanIds(DomainResource domainResource, string resultId)
        {
            var result = new List<string>();

            if (domainResource == null || string.IsNullOrWhiteSpace(resultId) || domainResource.Contained == null)
            {
                return result;
            }

            if (!(domainResource.Contained
                    .FirstOrDefault(contained => resultId.Equals(contained.Id, StringComparison.Ordinal)) is
                STU3Models.RequestGroup requestGroup))
            {
                return result;
            }

            foreach (var actionComponent in requestGroup.Action)
            {
                result.Add(actionComponent?.Resource?.Reference?.Substring(1));
            }

            return result;
        }

        private static void SanitizeReferralRequest(STU3Models.ReferralRequest referralRequest, IHtmlSanitizer htmlSanitizer)
        {
            if (referralRequest == null)
            {
                return;
            }

            referralRequest.Description = SanitizeAndDecodeHtml(referralRequest.Description, htmlSanitizer);
        }

        private static void SanitizeCarePlan(STU3Models.CarePlan carePlan, IHtmlSanitizer htmlSanitizer)
        {
            if (carePlan == null)
            {
                return;
            }

            carePlan.Title = SanitizeAndDecodeHtml(carePlan.Title, htmlSanitizer);
            SanitizeCarePlanActivities(carePlan.Activity, htmlSanitizer);
        }

        private static void SanitizeCarePlanActivities(IReadOnlyCollection<STU3Models.CarePlan.ActivityComponent> activityComponents, IHtmlSanitizer htmlSanitizer)
        {
            if (activityComponents == null)
            {
                return;
            }

            foreach (var activityComponent in activityComponents)
            {
                activityComponent.Detail.Description = SanitizeAndDecodeHtml(activityComponent.Detail?.Description, htmlSanitizer);
            }
        }

        private static void SanitizeQuestionnaires(DomainResource domainResource, IReadOnlyCollection<STU3Models.DataRequirement> dataRequirements, IHtmlSanitizer htmlSanitizer)
        {
            var questionnaireIds = GetQuestionnaireIdsFromDataRequirements(dataRequirements).ToList();

            SanitizeQuestionnaires(domainResource, questionnaireIds, htmlSanitizer);
        }

        private static IEnumerable<string> GetQuestionnaireIdsFromDataRequirements(IReadOnlyCollection<STU3Models.DataRequirement> dataRequirements)
        {
            var result = new List<string>();

            if (dataRequirements == null)
            {
                return result;
            }

            foreach (var dataRequirement in dataRequirements)
            {
                if (dataRequirement.Type != STU3Models.FHIRAllTypes.QuestionnaireResponse)
                {
                    continue;
                }

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
                if (!questionnaireIds.Contains(contained.Id))
                {
                    continue;
                }

                var questionnaire = contained as STU3Models.Questionnaire;
                SanitizeItems(questionnaire?.Item, htmlSanitizer);
            }
        }

        private static void SanitizeItems(List<STU3Models.Questionnaire.ItemComponent> items, IHtmlSanitizer htmlSanitizer)
        {
            if (items == null || items.Count == 0)
            {
                return;
            }

            items.ForEach(item =>
            {
                item.Text = SanitizeAndDecodeHtml(item.Text, htmlSanitizer);

                if (item.Option == null || item.Option.Count == 0)
                {
                    if (item.Item != null  && item.Item.Count > 0)
                    {
                        SanitizeItems(item.Item, htmlSanitizer);
                    }

                    return;

                }

                item.Option.ForEach(option =>
                {
                    var optionValue = (Coding) option.Value;
                    optionValue.Display = SanitizeAndDecodeHtml(optionValue.Display, htmlSanitizer);
                    option.Value = optionValue;
                });
            });
        }

        private static string SanitizeAndDecodeHtml(string html, IHtmlSanitizer htmlSanitizer)
        {
            html = htmlSanitizer.SanitizeHtml(html);
            return WebUtility.HtmlDecode(html);
        }
    }
}