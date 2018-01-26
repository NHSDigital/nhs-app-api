using System.Threading.Tasks;
using NHSOnline.Backend.Worker.Suppliers.Emis.Models;

namespace NHSOnline.Backend.Worker.Suppliers.Emis
{
    public interface IEmisClient
    {
        Task<CreateEndUserSessionResponseModel> EndUserSessionAsync();
        Task<DemographicsResponse> DemographicsAsync(string userPatientLinkToken, string responseSessionId, string endUserSessionId);
        Task<CreateSessionResponseModel> SessionsAsync(string endUserSessionId, string connectionToken, string odsCode);
    }
}
