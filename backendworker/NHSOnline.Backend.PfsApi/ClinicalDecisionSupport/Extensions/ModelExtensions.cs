extern alias stu3;

using Hl7.Fhir.Model;
using STU3Models = stu3::Hl7.Fhir.Model;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Extensions
{
    public static class ModelExtensions
    {
        public static bool TryDeriveResourceType(this Resource r, out STU3Models.ResourceType rt)
        {
            var resourceType = STU3Models.ModelInfo.FhirTypeNameToResourceType(r.TypeName);
            rt = resourceType.GetValueOrDefault(STU3Models.ResourceType.Account);
            return resourceType.HasValue;
        }
    }
}