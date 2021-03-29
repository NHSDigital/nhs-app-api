using System.Net.Http;
using System.Threading.Tasks;
using NHSOnline.App.NhsLogin.Fido.Models;

namespace NHSOnline.App.NhsLogin.Fido
{
    internal interface IUafClient
    {
        Task<HttpResponseMessage> GetRegistrationRequest(string accessToken);
        Task<HttpResponseMessage> PostRegistrationResponse(UafRegistrationResponse registrationResponse, string accessToken);
        Task<HttpResponseMessage> PostDeregistrationRequest(string accessToken, UafDeregistrationRequest request);
    }
}