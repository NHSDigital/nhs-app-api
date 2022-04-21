extern alias stu3;

using NHSOnline.Backend.Support.Sanitization;
using STU3Models = stu3::Hl7.Fhir.Model;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils
{
    public interface IFhirSanitizationHelper
    {
        void SanitizeGuidanceResponse(stu3::Hl7.Fhir.Model.GuidanceResponse guidanceResponse, IHtmlSanitizer htmlSanitizer);
        void SanitizeServiceDefinition(STU3Models.ServiceDefinition serviceDefinition, IHtmlSanitizer htmlSanitizer);
    }
}