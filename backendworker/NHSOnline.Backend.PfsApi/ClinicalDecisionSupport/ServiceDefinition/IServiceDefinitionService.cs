using System.Threading.Tasks;
using Hl7.Fhir.Model;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public interface IServiceDefinitionService
    {
        Task<ServiceDefinitionResult> GetServiceDefinitionById(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId);
        Task<ServiceDefinitionResult> EvaluateServiceDefinition(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId, Parameters parameters, bool addJavascriptDisabledHeader);
        Task<ServiceDefinitionResult> SearchServiceDefinitionsByQuery(IOnlineConsultationsProviderHttpClient httpClient);
    }
}