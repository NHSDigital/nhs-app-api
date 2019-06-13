using Hl7.Fhir.Model;
using NHSOnline.Backend.Support.Sanitization;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils
{
    public interface IFhirSanitizationHelper
    {
        void SanitizeGuidanceResponse(GuidanceResponse guidanceResponse, IHtmlSanitizer htmlSanitizer);
        void SanitizeServiceDefinition(Hl7.Fhir.Model.ServiceDefinition serviceDefinition, IHtmlSanitizer htmlSanitizer);
        void SanitizeServiceDefinitionSearchBundle(Bundle bundle, IHtmlSanitizer htmlSanitizer);
    }
}