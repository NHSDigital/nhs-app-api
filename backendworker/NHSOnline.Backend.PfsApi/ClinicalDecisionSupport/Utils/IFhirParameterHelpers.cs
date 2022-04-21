extern alias stu3;
using Hl7.Fhir.Model;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support.Session;
using NhsAppFhir = NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Models.Fhir;
using STU3Models = stu3::Hl7.Fhir.Model;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.Utils
{
    public interface IFhirParameterHelpers
    {
        STU3Models.Patient CreateFhirPatient(P9UserSession userSession, string address);
        Parameters RemoveServiceDefinitionMetadataFromParameters(Parameters parameters, out ServiceDefinitionMetaData metaData);
        NhsAppFhir.Parameters CreateInitialServiceDefinitionEvaluateParameters(string odsCode);
        Parameters CreateServiceDefinitionIsValidParameters(string odsCode, string requestId);
        string GetSessionIdFromParameters(Parameters parameters);
    }
}