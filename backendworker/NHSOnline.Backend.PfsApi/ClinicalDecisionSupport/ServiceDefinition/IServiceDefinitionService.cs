using System.Threading.Tasks;
using Hl7.Fhir.Model;
using NHSOnline.Backend.GpSystems.Demographics;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients;
using NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition.Models;
using NHSOnline.Backend.Support;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.ServiceDefinition
{
    public interface IServiceDefinitionService
    {
        Task<ServiceDefinitionResult> GetServiceDefinitionById(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId, string provider);
        Task<ServiceDefinitionResult> EvaluateServiceDefinition(IOnlineConsultationsProviderHttpClient httpClient, string serviceDefinitionId, Parameters parameters, bool addJavascriptDisabledHeader, UserSession userSession);
        Task<ServiceDefinitionResult> SearchServiceDefinitionsByQuery(IOnlineConsultationsProviderHttpClient httpClient);
    }
}