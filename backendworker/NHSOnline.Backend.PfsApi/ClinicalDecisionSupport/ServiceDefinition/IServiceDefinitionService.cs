using System.Threading.Tasks;
using Hl7.Fhir.Model;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public interface IServiceDefinitionService
    {
        Task<ServiceDefinitionResult> GetServiceDefinitionById(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId, string provider, UserSession userSession);
        Task<ServiceDefinitionResult> EvaluateServiceDefinition(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId, Parameters parameters, bool addJavascriptDisabledHeader, bool demographicsConsentGiven, UserSession userSession);
        Task<ServiceDefinitionListResult> GetServiceDefinitions(IOnlineConsultationsProviderHttpClient httpClient);
    }
}