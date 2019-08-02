using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public interface IOnlineConsultationsProviderHttpClient
    {
        Task<HttpResponseMessage> GetServiceDefinitionById(string serviceDefinitionId);
        Task<HttpResponseMessage> SearchServiceDefinitionsByQuery();
        Task<HttpResponseMessage> EvaluateServiceDefinition(string serviceDefinitionId, string requestBody, bool addJavascriptDisabledHeader);
    }
}