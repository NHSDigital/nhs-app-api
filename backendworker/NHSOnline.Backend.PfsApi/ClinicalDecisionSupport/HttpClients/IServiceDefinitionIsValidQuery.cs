using System.Net.Http;
using System.Threading.Tasks;

namespace NHSOnline.Backend.PfsApi.ClinicalDecisionSupport.HttpClients
{
    public interface IServiceDefinitionIsValidQuery
    {
        Task<HttpResponseMessage> ServiceDefinitionIsValid(string providerKey, string requestBody);
    }
}