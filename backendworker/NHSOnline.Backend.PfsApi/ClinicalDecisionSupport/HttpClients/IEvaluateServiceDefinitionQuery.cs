using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public interface IEvaluateServiceDefinitionQuery {
        Task<HttpResponseMessage> EvaluateServiceDefinition(string providerKey, string serviceDefinitionId, string requestBody, bool addJavascriptDisabledHeader);
    }
}