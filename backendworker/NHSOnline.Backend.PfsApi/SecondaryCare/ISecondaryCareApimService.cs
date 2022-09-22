using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.NHSApim;
using NHSOnline.Backend.PfsApi.NHSApim.Models;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public interface ISecondaryCareApimService
    {
        Task<(NhsApimAuthResponse<ApimAccessToken> authToken, bool isSuccess)> GetAuthToken(P9UserSession userSession, string operation);
    }
}