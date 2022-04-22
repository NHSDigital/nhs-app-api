using System.Threading.Tasks;
using NHSOnline.Backend.PfsApi.NHSApim.Models;

namespace NHSOnline.Backend.PfsApi.NHSApim
{
    public interface INhsApimClient
    {
        Task<NhsApimAuthResponse<ApimAccessToken>> GetAuthToken(string nhsLoginIdToken);
    }
}