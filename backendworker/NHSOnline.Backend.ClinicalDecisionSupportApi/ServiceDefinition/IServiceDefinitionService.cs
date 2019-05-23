using System.Threading.Tasks;
using Hl7.Fhir.Model;
using NHSOnline.Backend.ClinicalDecisionSupportApi.HttpClients;
using NHSOnline.Backend.ClinicalDecisionSupportApi.ServiceDefinition.Models;

namespace NHSOnline.Backend.ClinicalDecisionSupportApi.ServiceDefinition
{
    public interface IServiceDefinitionService
    {
        Task<ServiceDefinitionResult> GetServiceDefinitionById(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId);
        Task<ServiceDefinitionResult> EvaluateServiceDefinition(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId, Parameters parameters);
        Task<ServiceDefinitionResult> SearchServiceDefinitionsByQuery(IOnlineConsultationsProviderHttpClient httpClient);
    }
}