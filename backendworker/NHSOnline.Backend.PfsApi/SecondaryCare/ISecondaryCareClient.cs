using System.Threading.Tasks;
using NHSOnline.Backend.Support.Session;

namespace NHSOnline.Backend.PfsApi.SecondaryCare
{
    public interface ISecondaryCareClient
    {
        Task<SecondaryCareResponse> GetResponse(P9UserSession userSession, string accessToken, string path);
    }
}