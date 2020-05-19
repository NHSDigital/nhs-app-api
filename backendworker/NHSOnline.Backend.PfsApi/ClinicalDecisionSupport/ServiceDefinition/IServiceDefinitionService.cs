using System.Threading.Tasks;
using Hl7.Fhir.Model;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public interface IServiceDefinitionService
    {
        Task<ServiceDefinitionResult> GetServiceDefinition(string providerKey, string serviceDefinitionId, string serviceDefinitionDescription, P9UserSession userSession);
        Task<ServiceDefinitionResult> EvaluateServiceDefinition(string providerKey, string serviceDefinitionId, string serviceDefinitionDescription, Parameters parameters, bool addJavascriptDisabledHeader, bool demographicsConsentGiven, P9UserSession userSession);
        Task<ServiceDefinitionIsValidResult> GetServiceDefinitionIsValid(string providerKey, P9UserSession userSession);
        ServiceDefinitionResult GetProviderName(string providerKey);
    }
}